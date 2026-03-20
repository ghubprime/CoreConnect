using CoreConnect.Server.Data;
using CoreConnect.Shared.Entities;
using CoreConnect.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoreConnect.Server.Services;

public interface IAlertRuleProcessor
{
    Task EvaluateDeviceAsync(Device device);
}

public class AlertRuleProcessor : IAlertRuleProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AlertRuleProcessor> _logger;

    public AlertRuleProcessor(IServiceScopeFactory scopeFactory, IMemoryCache memoryCache, ILogger<AlertRuleProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task EvaluateDeviceAsync(Device device)
    {
        try
        {
            if (device == null || string.IsNullOrWhiteSpace(device.OrganizationID))
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDb>();
            var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();

            var rules = await dbContext.AlertRules
                .Where(r => r.OrganizationID == device.OrganizationID && r.IsEnabled)
                .AsNoTracking()
                .ToListAsync();

            if (!rules.Any())
            {
                return;
            }

            foreach (var rule in rules)
            {
                // Group check
                if (!string.IsNullOrWhiteSpace(rule.TargetDeviceGroupId) && 
                    device.DeviceGroupID != rule.TargetDeviceGroupId)
                {
                    continue;
                }

                double deviceValue = 0;
                if (rule.Metric.Equals("CpuUtilization", StringComparison.OrdinalIgnoreCase))
                {
                    deviceValue = device.CpuUtilization;
                }
                else if (rule.Metric.Equals("UsedMemoryPercent", StringComparison.OrdinalIgnoreCase))
                {
                    deviceValue = device.UsedMemoryPercent * 100;
                }
                else if (rule.Metric.Equals("UsedStoragePercent", StringComparison.OrdinalIgnoreCase))
                {
                    deviceValue = device.UsedStoragePercent * 100;
                }
                else
                {
                    continue;
                }

                bool isTriggered = false;
                switch (rule.Operator)
                {
                    case ">": isTriggered = deviceValue > rule.Threshold; break;
                    case "<": isTriggered = deviceValue < rule.Threshold; break;
                    case ">=": isTriggered = deviceValue >= rule.Threshold; break;
                    case "<=": isTriggered = deviceValue <= rule.Threshold; break;
                    case "==": isTriggered = Math.Abs(deviceValue - rule.Threshold) < 0.1; break;
                }

                if (isTriggered)
                {
                    string cacheKey = $"AlertRule_{rule.ID}_{device.ID}";
                    if (_memoryCache.TryGetValue(cacheKey, out _))
                    {
                        // Don't fire more than once an hour per rule per device
                        continue;
                    }

                    _memoryCache.Set(cacheKey, true, TimeSpan.FromHours(1));

                    string msg = $"Auto-Remediation Triggered: {rule.Name} on {device.DeviceName}. {rule.Metric} was {deviceValue:N1} {rule.Operator} {rule.Threshold}";
                    _logger.LogWarning(msg);

                    await dataService.AddAlert(device.ID, device.OrganizationID, msg);

                    if (rule.SavedScriptId.HasValue)
                    {
                        var attachDevice = await dbContext.Devices.FirstOrDefaultAsync(x => x.ID == device.ID);
                        if (attachDevice != null)
                        {
                            var scriptRun = new ScriptRun
                            {
                                OrganizationID = device.OrganizationID,
                                RunAt = DateTimeOffset.Now,
                                SavedScriptId = rule.SavedScriptId.Value,
                                RunOnNextConnect = true,
                                InputType = ScriptInputType.OneTimeScript,
                                Initiator = "Auto-Remediation System",
                                IsAutoRemediation = true,
                                Devices = new List<Device> { attachDevice }
                            };
                            
                            await dataService.AddScriptRun(scriptRun);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating alert rules for device {DeviceId}", device?.ID);
        }
    }
}

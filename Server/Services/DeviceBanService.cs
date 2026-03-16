using CoreConnect.Server.Models;

namespace CoreConnect.Server.Services;

public interface IDeviceBanService
{
    Task<bool> IsBanned(params string[] identifiers);
}

public class DeviceBanService : IDeviceBanService
{
    private readonly IDataService _dataService;
    private readonly ILogger<DeviceBanService> _logger;

    public DeviceBanService(IDataService dataService, ILogger<DeviceBanService> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<bool> IsBanned(params string[] identifiers)
    {
        var settings = await _dataService.GetSettings();
        foreach (var identifier in identifiers)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                continue;
            }

            if (settings.BannedDevices.Any(x => !string.IsNullOrWhiteSpace(x) &&
                x.Equals(identifier.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Device identifier ({identifier}) is banned.", identifier);
                return true;
            }
        }

        return false;
    }
}

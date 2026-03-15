using System.Diagnostics;
using CoreConnect.Server.Options;
using CoreConnect.Server.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace CoreConnect.Server.Filters;

/// <summary>
/// SignalR hub filter that logs slow method invocations and records
/// metrics to the <see cref="IBackpressureMetrics"/> singleton.
/// </summary>
public class HubBackpressureFilter : IHubFilter
{
    private readonly IBackpressureMetrics _metrics;
    private readonly ILogger<HubBackpressureFilter> _logger;
    private readonly int _slowThresholdMs;

    public HubBackpressureFilter(
        IBackpressureMetrics metrics,
        IOptions<HubBackpressureOptions> options,
        ILogger<HubBackpressureFilter> logger)
    {
        _metrics = metrics;
        _logger = logger;
        _slowThresholdMs = options.Value.SlowInvocationThresholdMs;
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var hubName = invocationContext.Hub.GetType().Name;
        var methodName = invocationContext.HubMethodName;
        var sw = Stopwatch.StartNew();

        try
        {
            return await next(invocationContext);
        }
        finally
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > _slowThresholdMs)
            {
                _logger.LogWarning(
                    "Slow hub invocation: {HubName}.{MethodName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms). ConnectionId: {ConnectionId}",
                    hubName,
                    methodName,
                    sw.ElapsedMilliseconds,
                    _slowThresholdMs,
                    invocationContext.Context.ConnectionId);

                _metrics.RecordSlowInvocation(hubName, methodName, sw.ElapsedMilliseconds);
            }
        }
    }
}

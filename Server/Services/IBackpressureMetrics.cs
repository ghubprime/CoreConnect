using System.Collections.Concurrent;

namespace CoreConnect.Server.Services;

/// <summary>
/// Tracks aggregate backpressure statistics across all hub connections.
/// Registered as a singleton for diagnostics endpoint consumption.
/// </summary>
public interface IBackpressureMetrics
{
    void RecordDroppedFrames(string hubName, string connectionId, long count);
    void RecordSlowInvocation(string hubName, string methodName, long elapsedMs);
    BackpressureSnapshot GetSnapshot();
}

public sealed class BackpressureSnapshot
{
    public long TotalDroppedFrames { get; init; }
    public long TotalSlowInvocations { get; init; }
    public IReadOnlyDictionary<string, long> DroppedFramesByHub { get; init; } = new Dictionary<string, long>();
    public IReadOnlyDictionary<string, long> SlowInvocationsByHub { get; init; } = new Dictionary<string, long>();
}

public sealed class BackpressureMetrics : IBackpressureMetrics
{
    private readonly ConcurrentDictionary<string, long> _droppedByHub = new();
    private readonly ConcurrentDictionary<string, long> _slowByHub = new();
    private long _totalDropped;
    private long _totalSlow;

    public void RecordDroppedFrames(string hubName, string connectionId, long count)
    {
        Interlocked.Add(ref _totalDropped, count);
        _droppedByHub.AddOrUpdate(hubName, count, (_, existing) => existing + count);
    }

    public void RecordSlowInvocation(string hubName, string methodName, long elapsedMs)
    {
        Interlocked.Increment(ref _totalSlow);
        _slowByHub.AddOrUpdate(hubName, 1, (_, existing) => existing + 1);
    }

    public BackpressureSnapshot GetSnapshot()
    {
        return new BackpressureSnapshot
        {
            TotalDroppedFrames = Interlocked.Read(ref _totalDropped),
            TotalSlowInvocations = Interlocked.Read(ref _totalSlow),
            DroppedFramesByHub = new Dictionary<string, long>(_droppedByHub),
            SlowInvocationsByHub = new Dictionary<string, long>(_slowByHub)
        };
    }
}

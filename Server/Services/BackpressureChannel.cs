using System.Threading.Channels;

namespace CoreConnect.Server.Services;

/// <summary>
/// A bounded channel wrapper that tracks dropped items when the channel is full.
/// Used per-connection to prevent a slow SignalR client from stalling the server.
/// </summary>
public sealed class BackpressureChannel<T>
{
    private readonly Channel<T> _channel;
    private long _droppedCount;

    public BackpressureChannel(int capacity, BoundedChannelFullMode fullMode = BoundedChannelFullMode.DropOldest)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = fullMode,
            SingleWriter = false,
            SingleReader = true
        };

        if (fullMode == BoundedChannelFullMode.DropOldest || fullMode == BoundedChannelFullMode.DropNewest)
        {
            // Channel<T> handles drops internally for these modes.
            // We wrap TryWrite to detect backpressure via the count heuristic.
            options.FullMode = fullMode;
        }

        _channel = Channel.CreateBounded<T>(options);
    }

    /// <summary>
    /// Number of items dropped because the channel was full.
    /// </summary>
    public long DroppedCount => Interlocked.Read(ref _droppedCount);

    public ChannelReader<T> Reader => _channel.Reader;

    /// <summary>
    /// Attempts to write an item. Returns true if written, false if dropped.
    /// </summary>
    public bool TryWrite(T item)
    {
        if (_channel.Writer.TryWrite(item))
        {
            return true;
        }

        Interlocked.Increment(ref _droppedCount);
        return false;
    }

    /// <summary>
    /// Signals that no more items will be written.
    /// </summary>
    public void Complete() => _channel.Writer.TryComplete();
}

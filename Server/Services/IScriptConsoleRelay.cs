using System.Collections.Concurrent;
using System.Threading.Channels;

namespace CoreConnect.Server.Services;

/// <summary>
/// Represents a single line of script output for the live console.
/// </summary>
public sealed record ScriptOutputLine(int ScriptRunId, string DeviceId, string Chunk, bool IsError, DateTimeOffset Timestamp);

/// <summary>
/// Relay service for streaming script output from agents to Blazor circuits.
/// Agents write chunks; Blazor pages subscribe by ScriptRunId.
/// </summary>
public interface IScriptConsoleRelay
{
    /// <summary>
    /// Write a chunk of output from an agent.
    /// </summary>
    void WriteChunk(int scriptRunId, string deviceId, string chunk, bool isError);

    /// <summary>
    /// Subscribe to output for a given script run. Returns a ChannelReader the Blazor page consumes.
    /// </summary>
    ChannelReader<ScriptOutputLine> Subscribe(int scriptRunId);

    /// <summary>
    /// Unsubscribe and clean up resources for a script run.
    /// </summary>
    void Unsubscribe(int scriptRunId);
}

public sealed class ScriptConsoleRelay : IScriptConsoleRelay
{
    private readonly ConcurrentDictionary<int, Channel<ScriptOutputLine>> _channels = new();

    public void WriteChunk(int scriptRunId, string deviceId, string chunk, bool isError)
    {
        if (_channels.TryGetValue(scriptRunId, out var channel))
        {
            var line = new ScriptOutputLine(scriptRunId, deviceId, chunk, isError, DateTimeOffset.UtcNow);
            // Best-effort write; if the channel is full, we drop the oldest.
            channel.Writer.TryWrite(line);
        }
    }

    public ChannelReader<ScriptOutputLine> Subscribe(int scriptRunId)
    {
        var channel = _channels.GetOrAdd(scriptRunId, _ =>
            Channel.CreateBounded<ScriptOutputLine>(new BoundedChannelOptions(1024)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,
                SingleReader = true
            }));

        return channel.Reader;
    }

    public void Unsubscribe(int scriptRunId)
    {
        if (_channels.TryRemove(scriptRunId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }
}

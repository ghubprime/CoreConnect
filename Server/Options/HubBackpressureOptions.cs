using System.Threading.Channels;

namespace CoreConnect.Server.Options;

/// <summary>
/// Configuration for per-connection SignalR hub backpressure channels.
/// Bind from appsettings.json section "HubBackpressure".
/// </summary>
public class HubBackpressureOptions
{
    public const string SectionKey = "HubBackpressure";

    /// <summary>
    /// Maximum number of outbound frames buffered per connection before the
    /// <see cref="FullMode"/> policy kicks in.
    /// </summary>
    public int ChannelCapacity { get; set; } = 512;

    /// <summary>
    /// What happens when the channel is full.
    /// Default is <see cref="BoundedChannelFullMode.DropOldest"/> so the viewer
    /// always gets the most recent frame.
    /// </summary>
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.DropOldest;

    /// <summary>
    /// Hub method invocations slower than this threshold (ms) will be logged as warnings.
    /// </summary>
    public int SlowInvocationThresholdMs { get; set; } = 500;
}

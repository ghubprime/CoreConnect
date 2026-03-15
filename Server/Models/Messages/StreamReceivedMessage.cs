using System.Collections.Generic;

namespace CoreConnect.Server.Models.Messages;

public class StreamReceivedMessage<T>
{
    public IAsyncEnumerable<T> Stream { get; }

    public StreamReceivedMessage(IAsyncEnumerable<T> stream)
    {
        Stream = stream;
    }
}

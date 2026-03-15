using System.Runtime.Serialization;

namespace CoreConnect.Shared.Models;

[DataContract]
public class RemoteControlViewerOptions
{
    [DataMember]
    public bool ShouldRecordSession { get; init; }
}

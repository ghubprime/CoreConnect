using System.Runtime.Serialization;

namespace CoreConnect.Shared.Models.Dtos;

[DataContract]
public class FrameReceivedDto
{
    [DataMember]
    public long Timestamp { get; set; }
}

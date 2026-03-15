using System.Runtime.Serialization;

namespace CoreConnect.Shared.Models.Dtos;

[DataContract]
public class ToggleAudioDto
{
    [DataMember(Name = "ToggleOn")]
    public bool ToggleOn { get; set; }
}

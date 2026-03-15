using System.Runtime.Serialization;

namespace CoreConnect.Shared.Models.Dtos;

[DataContract]
public class KeyUpDto
{
    [DataMember(Name = "Key")]
    public string Key { get; set; } = string.Empty;

}

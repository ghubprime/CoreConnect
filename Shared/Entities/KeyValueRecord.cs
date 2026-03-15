using System.ComponentModel.DataAnnotations;

namespace CoreConnect.Shared.Entities;

public class KeyValueRecord
{
    [Key]
    public Guid Key { get; set; }
    public string? Value { get; set; }
}

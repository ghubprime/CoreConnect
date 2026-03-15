using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreConnect.Shared.Entities;

public class DeviceTelemetrySnapshot
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string DeviceId { get; set; } = null!;

    [JsonIgnore]
    public Device? Device { get; set; }

    public double CpuUtilization { get; set; }

    public double UsedMemoryPercent { get; set; }

    public double UsedStoragePercent { get; set; }

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}

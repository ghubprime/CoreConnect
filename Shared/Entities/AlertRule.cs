using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreConnect.Shared.Entities;

public class AlertRule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public string OrganizationID { get; set; } = null!;

    [JsonIgnore]
    public Organization? Organization { get; set; }

    /// <summary>
    /// E.g. CpuUtilization, UsedMemoryPercent, UsedStoragePercent
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Metric { get; set; } = null!;

    /// <summary>
    /// E.g. >, <, ==
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Operator { get; set; } = null!;

    [Required]
    public double Threshold { get; set; }

    public Guid? SavedScriptId { get; set; }

    [JsonIgnore]
    public SavedScript? SavedScript { get; set; }

    public string? TargetDeviceGroupId { get; set; }

    [JsonIgnore]
    public DeviceGroup? TargetDeviceGroup { get; set; }

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

    public bool IsEnabled { get; set; } = true;
}

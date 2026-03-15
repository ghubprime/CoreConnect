using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreConnect.Shared.Entities;

public class Organization
{
    public ICollection<Alert> Alerts { get; set; } = [];

    public ICollection<AlertRule> AlertRules { get; set; } = [];

    public ICollection<ApiToken> ApiTokens { get; set; } = [];

    public BrandingInfo? BrandingInfo { get; set; }
    public string? BrandingInfoId { get; set; }

    public ICollection<ScriptResult> ScriptResults { get; set; } = [];

    public ICollection<ScriptRun> ScriptRuns { get; set; } = [];
    public ICollection<SavedScript> SavedScripts { get; set; } = [];

    public ICollection<ScriptSchedule> ScriptSchedules { get; set; } = [];

    public ICollection<DeviceGroup> DeviceGroups { get; set; } = [];

    public ICollection<Device> Devices { get; set; } = [];

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    public ICollection<InviteLink> InviteLinks { get; set; } = [];

    public bool IsDefaultOrganization { get; set; }

    [StringLength(25)]
    public required string OrganizationName { get; set; }

    public ICollection<CoreConnectUser> CoreConnectUsers { get; set; } = [];
    public ICollection<SharedFile> SharedFiles { get; set; } = [];

    /// <summary>
    /// Optional list of CIDR ranges (e.g. "10.0.0.0/8", "192.168.1.0/24") that
    /// restrict API and SignalR access to this organization. Null or empty = no restriction.
    /// </summary>
    public string[]? AllowedIpRanges { get; set; }
}
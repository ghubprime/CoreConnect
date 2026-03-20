using CoreConnect.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreConnect.Shared.Entities;

public class ScriptRun
{
    [JsonIgnore]
    public List<Device>? Devices { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Initiator { get; set; }

    public ScriptInputType InputType { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;

    [JsonIgnore]
    public List<ScriptResult>? Results { get; set; }

    public DateTimeOffset RunAt { get; set; }
    public bool RunOnNextConnect { get; set; }

    [JsonIgnore]
    public SavedScript? SavedScript { get; set; }
    public Guid? SavedScriptId { get; set; }

    [JsonIgnore]
    public ScriptSchedule? Schedule { get; set; }
    public int? ScheduleId { get; set; }

    /// <summary>
    /// Indicates this script run was triggered automatically by the alert
    /// auto-remediation system rather than by a manual user action.
    /// </summary>
    public bool IsAutoRemediation { get; set; }
}

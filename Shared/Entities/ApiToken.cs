using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CoreConnect.Shared.Enums;

namespace CoreConnect.Shared.Entities;

public class ApiToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { get; set; } = null!;

    public DateTimeOffset? LastUsed { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public string OrganizationID { get; set; } = null!;
    public string? Secret { get; set; }

    /// <summary>
    /// Scoped permissions for this API token.
    /// Defaults to <see cref="ApiPermission.All"/> for backwards compatibility.
    /// </summary>
    public ApiPermission Permissions { get; set; } = ApiPermission.All;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CoreConnect.Shared.Entities;

public class UserCredential
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public CoreConnectUser? User { get; set; }

    /// <summary>
    /// The FIDO2 credential ID returned by the authenticator.
    /// </summary>
    [Required]
    public byte[] CredentialId { get; set; } = null!;

    /// <summary>
    /// The public key used to verify assertions.
    /// </summary>
    [Required]
    public byte[] PublicKey { get; set; } = null!;

    /// <summary>
    /// Counter for replay attack detection.
    /// </summary>
    public uint SignatureCounter { get; set; }

    public string CredType { get; set; } = string.Empty;

    public Guid AaGuid { get; set; }

    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

    [StringLength(100)]
    public string? DisplayName { get; set; }
}

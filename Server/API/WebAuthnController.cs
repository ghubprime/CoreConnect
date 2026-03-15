using CoreConnect.Server.Data;
using CoreConnect.Shared.Entities;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CoreConnect.Server.API;

[Route("api/[controller]")]
[ApiController]
public class WebAuthnController : ControllerBase
{
    private readonly IFido2 _fido2;
    private readonly IAppDbFactory _dbFactory;
    private readonly UserManager<CoreConnectUser> _userManager;
    private readonly SignInManager<CoreConnectUser> _signInManager;
    private readonly ILogger<WebAuthnController> _logger;

    public WebAuthnController(
        IFido2 fido2,
        IAppDbFactory dbFactory,
        UserManager<CoreConnectUser> userManager,
        SignInManager<CoreConnectUser> signInManager,
        ILogger<WebAuthnController> logger)
    {
        _fido2 = fido2;
        _dbFactory = dbFactory;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    /// <summary>
    /// Returns credential creation options for the browser's navigator.credentials.create() API.
    /// </summary>
    [Authorize]
    [HttpPost("register/options")]
    public async Task<IActionResult> RegisterOptions()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        using var db = _dbFactory.GetContext();
        var existingCreds = await db.UserCredentials
            .Where(x => x.UserId == user.Id)
            .Select(x => new PublicKeyCredentialDescriptor(x.CredentialId))
            .ToListAsync();

        var fidoUser = new Fido2User
        {
            Id = Encoding.UTF8.GetBytes(user.Id),
            Name = user.UserName ?? user.Email ?? user.Id,
            DisplayName = user.UserName ?? user.Email ?? user.Id
        };

        var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = fidoUser,
            ExcludeCredentials = existingCreds,
            AuthenticatorSelection = AuthenticatorSelection.Default,
            AttestationPreference = AttestationConveyancePreference.None
        });

        HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());
        return Ok(options);
    }

    /// <summary>
    /// Completes registration by validating the attestation response and persisting the credential.
    /// </summary>
    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthenticatorAttestationRawResponse attestation)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
        if (string.IsNullOrEmpty(jsonOptions)) return BadRequest("Session expired. Please start over.");

        var options = CredentialCreateOptions.FromJson(jsonOptions);

        var credential = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
        {
            AttestationResponse = attestation,
            OriginalOptions = options,
            IsCredentialIdUniqueToUserCallback = async (args, _) =>
            {
                using var innerDb = _dbFactory.GetContext();
                // Load all credential IDs for comparison (cannot use SequenceEqual in EF LINQ).
                var credIds = await innerDb.UserCredentials
                    .Select(c => c.CredentialId)
                    .ToListAsync();
                return !credIds.Any(id => id.AsSpan().SequenceEqual(args.CredentialId));
            }
        });

        // In Fido2NetLib v4, MakeNewCredentialAsync returns RegisteredPublicKeyCredential directly.
        using var db = _dbFactory.GetContext();
        db.UserCredentials.Add(new UserCredential
        {
            UserId = user.Id,
            CredentialId = credential.Id,
            PublicKey = credential.PublicKey,
            SignatureCounter = credential.SignCount,
            CredType = credential.Type.ToString(),
            AaGuid = credential.AaGuid,
            DisplayName = $"Passkey {DateTimeOffset.Now:yyyy-MM-dd}"
        });
        await db.SaveChangesAsync();

        return Ok(new { status = "ok" });
    }

    /// <summary>
    /// Returns assertion options for the browser's navigator.credentials.get() API.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login/options")]
    public async Task<IActionResult> LoginOptions([FromBody] LoginOptionsRequest? request)
    {
        using var db = _dbFactory.GetContext();

        List<PublicKeyCredentialDescriptor> allowedCreds = new();

        if (!string.IsNullOrEmpty(request?.UserName))
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is not null)
            {
                allowedCreds = await db.UserCredentials
                    .Where(x => x.UserId == user.Id)
                    .Select(x => new PublicKeyCredentialDescriptor(x.CredentialId))
                    .ToListAsync();
            }
        }

        var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = allowedCreds,
            UserVerification = UserVerificationRequirement.Preferred
        });

        HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());
        return Ok(options);
    }

    /// <summary>
    /// Validates the assertion response and signs the user in.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticatorAssertionRawResponse assertion)
    {
        var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
        if (string.IsNullOrEmpty(jsonOptions)) return BadRequest("Session expired. Please start over.");

        var options = AssertionOptions.FromJson(jsonOptions);

        using var db = _dbFactory.GetContext();

        // assertion.Id is a base64url string in Fido2 v4; decode to bytes for comparison.
        var assertionIdBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(assertion.Id);
        var allCreds = await db.UserCredentials.ToListAsync();
        var storedCred = allCreds.FirstOrDefault(x =>
            x.CredentialId.AsSpan().SequenceEqual(assertionIdBytes));

        if (storedCred is null) return Unauthorized("Credential not registered.");

        var result = await _fido2.MakeAssertionAsync(new MakeAssertionParams
        {
            AssertionResponse = assertion,
            OriginalOptions = options,
            StoredPublicKey = storedCred.PublicKey,
            StoredSignatureCounter = storedCred.SignatureCounter,
            IsUserHandleOwnerOfCredentialIdCallback = async (args, _) =>
            {
                using var innerDb = _dbFactory.GetContext();
                var creds = await innerDb.UserCredentials.ToListAsync();
                return creds.Any(c => c.CredentialId.AsSpan().SequenceEqual(args.CredentialId));
            }
        });

        // Update signature counter.
        storedCred.SignatureCounter = result.SignCount;
        db.UserCredentials.Update(storedCred);
        await db.SaveChangesAsync();

        // Sign the user in.
        var user = await _userManager.FindByIdAsync(storedCred.UserId);
        if (user is null) return Unauthorized();

        await _signInManager.SignInAsync(user, isPersistent: true);
        return Ok(new { status = "ok" });
    }

    public class LoginOptionsRequest
    {
        public string? UserName { get; set; }
    }
}

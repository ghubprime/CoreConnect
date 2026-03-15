using System.Net;
using System.Security.Claims;
using CoreConnect.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace CoreConnect.Server.Middleware;

/// <summary>
/// Middleware that enforces organization-level IP allow-lists.
/// Runs after authentication so the user's organization is known.
/// Server admins and unauthenticated requests bypass the check.
/// </summary>
public class IpAllowListMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpAllowListMiddleware> _logger;

    public IpAllowListMiddleware(RequestDelegate next, ILogger<IpAllowListMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDb dbContext)
    {
        // Only enforce for authenticated users.
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // Server admins bypass the allow-list.
        if (context.User.IsInRole("ServerAdmin"))
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Organization?.AllowedIpRanges is not { Length: > 0 } ranges)
        {
            // No allow-list configured — allow all.
            await _next(context);
            return;
        }

        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is null)
        {
            _logger.LogWarning("Request has no remote IP address. Denying access for user {UserId}.", userId);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden: unable to determine client IP.");
            return;
        }

        // Map IPv6-mapped IPv4 addresses to their IPv4 form for matching.
        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        if (IsIpAllowed(remoteIp, ranges))
        {
            await _next(context);
            return;
        }

        _logger.LogWarning(
            "IP {RemoteIp} is not in the allow-list for organization {OrgId}. User: {UserId}.",
            remoteIp,
            user.OrganizationID,
            userId);

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Forbidden: your IP address is not in the organization allow-list.");
    }

    /// <summary>
    /// Checks whether <paramref name="clientIp"/> falls within any of the
    /// provided CIDR ranges or exact IP addresses.
    /// </summary>
    internal static bool IsIpAllowed(IPAddress clientIp, string[] allowedRanges)
    {
        foreach (var range in allowedRanges)
        {
            var trimmed = range.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                continue;
            }

            // Try CIDR notation first (e.g. "10.0.0.0/8").
            if (trimmed.Contains('/'))
            {
                if (IPNetwork.TryParse(trimmed, out var network) && network.Contains(clientIp))
                {
                    return true;
                }
            }
            else
            {
                // Exact IP match.
                if (IPAddress.TryParse(trimmed, out var exactIp) && exactIp.Equals(clientIp))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

using CoreConnect.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CoreConnect.Server.Auth;

/// <summary>
/// Action filter that enforces scoped API token permissions.
/// Reads <see cref="ApiPermission"/> from <c>HttpContext.Items["ApiPermissions"]</c>,
/// which is populated by <see cref="ApiAuthorizationFilter"/>.
/// If the token does not have the required permission, returns 403 Forbidden.
/// Cookie-authenticated admin users bypass this check.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class RequireApiPermissionAttribute : Attribute, IAsyncActionFilter
{
    private readonly ApiPermission _required;

    public RequireApiPermissionAttribute(ApiPermission required)
    {
        _required = required;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Cookie-authenticated admin users are not subject to API token scoping.
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            return next();
        }

        if (context.HttpContext.Items.TryGetValue("ApiPermissions", out var permObj) &&
            permObj is ApiPermission granted)
        {
            if ((granted & _required) == _required)
            {
                return next();
            }
        }

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Result = new JsonResult(new { error = $"API token lacks required permission: {_required}" })
        {
            StatusCode = (int)HttpStatusCode.Forbidden
        };

        return Task.CompletedTask;
    }
}

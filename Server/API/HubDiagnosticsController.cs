using CoreConnect.Server.Auth;
using CoreConnect.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreConnect.Server.API;

[Route("api/diagnostics")]
[ApiController]
[Authorize(Policy = PolicyNames.ServerAdminRequired)]
public class HubDiagnosticsController : ControllerBase
{
    private readonly IBackpressureMetrics _metrics;

    public HubDiagnosticsController(IBackpressureMetrics metrics)
    {
        _metrics = metrics;
    }

    /// <summary>
    /// Returns aggregate hub backpressure statistics including dropped frames
    /// and slow invocation counts per hub.
    /// </summary>
    [HttpGet("hub-pressure")]
    public ActionResult<BackpressureSnapshot> GetHubPressure()
    {
        return Ok(_metrics.GetSnapshot());
    }
}

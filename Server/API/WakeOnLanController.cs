using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CoreConnect.Server.Auth;
using CoreConnect.Server.Extensions;
using CoreConnect.Server.Hubs;
using CoreConnect.Server.Services;
using CoreConnect.Shared.Enums;
using CoreConnect.Shared.Interfaces;

namespace CoreConnect.Server.API;

[ApiController]
[Route("api/[controller]")]
public class WakeOnLanController : ControllerBase
{
    private readonly IDataService _dataService;
    private readonly IAgentHubSessionCache _agentSessionCache;
    private readonly IHubContext<AgentHub, IAgentHubClient> _agentHubContext;
    private readonly ILogger<WakeOnLanController> _logger;

    public WakeOnLanController(
        IDataService dataService,
        IAgentHubSessionCache agentSessionCache,
        IHubContext<AgentHub, IAgentHubClient> agentHubContext,
        ILogger<WakeOnLanController> logger)
    {
        _dataService = dataService;
        _agentSessionCache = agentSessionCache;
        _agentHubContext = agentHubContext;
        _logger = logger;
    }

    [HttpPost("{deviceId}")]
    [ServiceFilter(typeof(ApiAuthorizationFilter))]
    [RequireApiPermission(ApiPermission.WakeOnLan)]
    public async Task<IActionResult> Wake(string deviceId)
    {
        if (!Request.Headers.TryGetOrganizationId(out var orgId))
        {
            return Unauthorized();
        }

        var deviceResult = await _dataService.GetDevice(orgId, deviceId);

        if (!deviceResult.IsSuccess)
        {
            return NotFound("Device not found.");
        }

        var device = deviceResult.Value;

        if (device.MacAddresses is null || device.MacAddresses.Length == 0)
        {
            return BadRequest("Device has no MAC addresses stored.");
        }

        // Find peer devices in the same org that can relay the WoL magic packet.
        var peerDevices = _agentSessionCache
            .GetAllDevices()
            .Where(x =>
                x.OrganizationID == orgId &&
                (x.DeviceGroupID == device.DeviceGroupID || x.PublicIP == device.PublicIP));

        var sentCount = 0;

        foreach (var peer in peerDevices)
        {
            if (!_agentSessionCache.TryGetConnectionId(peer.ID, out var connectionId))
            {
                continue;
            }

            foreach (var mac in device.MacAddresses)
            {
                _logger.LogInformation(
                    "API: Sending WoL for device {deviceName} ({deviceId}) via peer {peerName} ({peerId}). MAC: {mac}",
                    device.DeviceName,
                    device.ID,
                    peer.DeviceName,
                    peer.ID,
                    mac);

                await _agentHubContext.Clients.Client(connectionId).WakeDevice(mac);
                sentCount++;
            }
        }

        if (sentCount == 0)
        {
            return Conflict("No online peer devices available to relay the wake command.");
        }

        return Ok($"Wake command sent via {sentCount} peer relay(s).");
    }
}

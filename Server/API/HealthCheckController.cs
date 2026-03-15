using Microsoft.AspNetCore.Mvc;
using CoreConnect.Server.Auth;
using CoreConnect.Server.Services;

namespace CoreConnect.Server.API;

/// <summary>
/// Can only be accessed from the local machine.  The sole purpose
/// is to provide a healthcheck endpoint for Docker that exercises
/// the database connection.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[ServiceFilter(typeof(LocalOnlyFilter))]
public class HealthCheckController : ControllerBase
{
    private readonly IDataService _dataService;

    public HealthCheckController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
         _ = await _dataService.GetOrganizationCountAsync();
        return NoContent();
    }
}

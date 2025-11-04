using DashboardServiceOptimization.Api.Services.Dashboard;
using DashboardServiceOptimization.Api.Services.DashboardCaches;
using Microsoft.AspNetCore.Mvc;

namespace DashboardServiceOptimization.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardCache _cache;
    private readonly IDashboardService _service;
    private readonly ILogger<DashboardController> _logger;
    private const string CacheKey = "dashboard:main";


    public DashboardController(IDashboardCache cache, IDashboardService service, ILogger<DashboardController> logger)
    {
        _cache = cache;
        _service = service;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var dto = await _cache.GetOrCreateAsync(CacheKey, ct => _service.ProduceAsync(ct), cancellationToken);
            return Ok(dto);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499); // client closed request
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve dashboard");
            return StatusCode(500, "Failed to produce dashboard data");
        }
    }
}

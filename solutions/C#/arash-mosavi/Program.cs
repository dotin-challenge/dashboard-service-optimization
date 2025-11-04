using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMemoryCache();


builder.Services.AddSingleton<IDashboardService, DashboardService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


public interface IDashboardService
{
    Task<DashboardData> GetDashboardDataAsync();
}


public class DashboardService : IDashboardService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DashboardService> _logger;

    
    private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

    private const string CacheKey = "DashboardData";

    
    private static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromHours(1);  
    private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(30); 

    public DashboardService(IMemoryCache memoryCache, ILogger<DashboardService> logger)
    {
        
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<DashboardData> GetDashboardDataAsync()
    {

        if (_memoryCache.TryGetValue(CacheKey, out DashboardData? cachedData) && cachedData != null)
        {
            _logger.LogInformation("Cache HIT");
            return cachedData;
        }

        
        _logger.LogWarning("Cache MISS");

        
        await _cacheLock.WaitAsync();
        try
        {

            if (_memoryCache.TryGetValue(CacheKey, out cachedData) && cachedData != null)
            {
                _logger.LogInformation("Cache HIT after lock!");
                return cachedData;
            }

            
            _logger.LogWarning("Regenerating dashboard data");

            var stopwatch = Stopwatch.StartNew();
            var freshData = await GenerateDashboardDataAsync();
            stopwatch.Stop();

            _logger.LogWarning($"Dashboard data regenerated in {stopwatch.ElapsedMilliseconds}ms");

            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(SlidingExpiration)  
                .SetAbsoluteExpiration(AbsoluteExpiration) 
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _logger.LogInformation($"Cache evicted. Reason: {reason}");
                });

            _memoryCache.Set(CacheKey, freshData, cacheOptions);

            return freshData;
        }
        finally
        {
            _cacheLock.Release();
            _logger.LogInformation("Lock released");
        }
    }

    private async Task<DashboardData> GenerateDashboardDataAsync()
    {
        
        await Task.Delay(TimeSpan.FromSeconds(3));

        
        return new DashboardData
        {
            GeneratedAt = DateTime.UtcNow,
            TotalUsers = Random.Shared.Next(10000, 50000),
            ActiveSessions = Random.Shared.Next(500, 5000),
            TotalRevenue = Random.Shared.Next(100000, 1000000),
            AverageResponseTime = Random.Shared.Next(100, 500),
            Metrics = new Dictionary<string, object>
            {
                { "CpuUsage", Random.Shared.Next(20, 80) + "%" },
                { "MemoryUsage", Random.Shared.Next(40, 90) + "%" },
                { "RequestsPerSecond", Random.Shared.Next(100, 1000) },
                { "ErrorRate", Random.Shared.NextDouble() * 5 + "%" },
                { "DatabaseConnections", Random.Shared.Next(10, 100) }
            }
        };
    }
}


[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardData), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var data = await _dashboardService.GetDashboardDataAsync();

            return Ok(new
            {
                Success = true,
                Data = data,
                RequestedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return StatusCode(500, new { Success = false, Error = "Internal server error" });
        }
    }
}


public class DashboardData
{
    public DateTime GeneratedAt { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveSessions { get; set; }
    public decimal TotalRevenue { get; set; }
    public int AverageResponseTime { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}

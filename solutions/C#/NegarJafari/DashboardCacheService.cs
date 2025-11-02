using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class DashboardCacheService
{
    private const string CacheKey = "dashboard:data";
    private readonly IMemoryCache _cache;
    private readonly ILogger<DashboardCacheService> _logger;

    // برای جلوگیری از چند بازسازیِ همزمان: key -> Lazy<Task<DashboardDto>>
    private readonly ConcurrentDictionary<string, Lazy<Task<DashboardDto>>> _rebuildTasks
        = new();

    // پیکربندی‌ها
    private readonly TimeSpan _initialAbsoluteExpiration = TimeSpan.FromHours(1); // ذخیره اولیه: 1 ساعت
    private readonly TimeSpan _readExtension = TimeSpan.FromMinutes(30);         // هر خواندن: تمدید 30 دقیقه
    private readonly TimeSpan _refreshAheadThreshold = TimeSpan.FromMinutes(2);  // اگر زمان باقیمانده کمتر از 2 دقیقه بود، refresh-ahead شروع می‌شود

    public DashboardCacheService(IMemoryCache cache, ILogger<DashboardCacheService> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        // Try to get from cache
        CachedDashboardData? cachedData = null;
        if (_cache.TryGetValue(CacheKey, out CachedDashboardData? tempData) && tempData != null)
        {
            cachedData = tempData;
            var timeRemaining = cachedData.AbsoluteExpiration - DateTimeOffset.UtcNow;
            
            // Extend cache lifetime by 30 minutes if accessed before expiry
            if (timeRemaining > TimeSpan.Zero)
            {
                _logger.LogInformation("Cache hit - extending expiration by 30 minutes");
                var newExpiration = DateTimeOffset.UtcNow.Add(_readExtension);
                
                _cache.Set(CacheKey, new CachedDashboardData
                {
                    Data = cachedData.Data,
                    AbsoluteExpiration = newExpiration
                }, newExpiration);
                
                return cachedData.Data;
            }
        }

        // Cache miss or expired - use Lazy<Task> to prevent cache stampede
        var rebuildTask = _rebuildTasks.GetOrAdd(CacheKey, key => new Lazy<Task<DashboardDto>>(
            async () =>
            {
                try
                {
                    _logger.LogInformation("Cache miss - generating new dashboard data");
                    
                    var dashboardData = await GenerateDashboardDataAsync(cancellationToken);
                    
                    // Cache for 1 hour
                    var expiration = DateTimeOffset.UtcNow.Add(_initialAbsoluteExpiration);
                    _cache.Set(CacheKey, new CachedDashboardData
                    {
                        Data = dashboardData,
                        AbsoluteExpiration = expiration
                    }, expiration);
                    
                    _logger.LogInformation("Dashboard data generated and cached");
                    
                    return dashboardData;
                }
                finally
                {
                    // Remove the rebuild task after completion
                    _rebuildTasks.TryRemove(CacheKey, out _);
                }
            },
            LazyThreadSafetyMode.ExecutionAndPublication
        ));

        try
        {
            // If there's stale cached data (expired but still in cache), return it while regeneration happens
            if (cachedData != null)
            {
                _logger.LogInformation("Returning stale data while regeneration in progress");
                // Start regeneration in background (fire and forget) - Lazy ensures only one execution
                _ = Task.Run(async () => await rebuildTask.Value, cancellationToken);
                return cachedData.Data;
            }

            // No stale data available - wait for the regeneration task to complete
            return await rebuildTask.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during dashboard data regeneration");
            throw;
        }
    }

    private async Task<DashboardDto> GenerateDashboardDataAsync(CancellationToken cancellationToken)
    {
        // Simulate complex analytical computation
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        
        return new DashboardDto
        {
            TotalUsers = 12500,
            ActiveSessions = 342,
            Revenue = 45678.90m,
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }

    private class CachedDashboardData
    {
        public DashboardDto Data { get; set; } = null!;
        public DateTimeOffset AbsoluteExpiration { get; set; }
    }
}

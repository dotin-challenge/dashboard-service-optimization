using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace DashboardCacheService.Services;

public interface IDashboardServiceCached
{
    Task<DashboardData> GetDashboardDataAsync(CancellationToken cancellationToken = default);
}

public class DashboardServiceCached : IDashboardServiceCached
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<DashboardServiceCached> _logger;
    private readonly SemaphoreSlim _lock;

    private const string CacheKey = "DashboardData";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan ExtensionDuration = TimeSpan.FromMinutes(30);

    public DashboardServiceCached(IMemoryCache cache, ILogger<DashboardServiceCached> logger)
    {
        _cache = cache;
        _logger = logger;
        _lock = new SemaphoreSlim(1, 1);
    }

    public async Task<DashboardData> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        // Check cache first
        if (_cache.TryGetValue(CacheKey, out DashboardData? data) && data != null)
        {
            _logger.LogInformation("Cache hit, extending lifetime");
            ExtendCacheLifetime(data);
            return data;
        }

        _logger.LogWarning("Cache miss, generating data");

        // Prevent multiple threads from regenerating simultaneously
        await _lock.WaitAsync(cancellationToken);

        try
        {
            // Double-check maybe another thread just populated the cache
            if (_cache.TryGetValue(CacheKey, out data) && data != null)
            {
                _logger.LogInformation("Data available from another thread");
                ExtendCacheLifetime(data);
                return data;
            }

            // Generate fresh data
            var stopwatch = Stopwatch.StartNew();
            data = await GenerateDataAsync(cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation("Data generated in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            // Cache it
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                Priority = CacheItemPriority.High
            };

            _cache.Set(CacheKey, data, options);
            return data;
        }
        finally
        {
            _lock.Release();
        }
    }

    private void ExtendCacheLifetime(DashboardData data)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ExtensionDuration,
            Priority = CacheItemPriority.High
        };

        _cache.Set(CacheKey, data, options);
    }

    private async Task<DashboardData> GenerateDataAsync(CancellationToken cancellationToken)
    {
        // Simulate heavy computation
        await Task.Delay(3000, cancellationToken);

        return new DashboardData
        {
            TotalUsers = Random.Shared.Next(10000, 50000),
            ActiveSessions = Random.Shared.Next(100, 1000),
            Revenue = Random.Shared.Next(100000, 500000),
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }
}

public record DashboardData
{
    public int TotalUsers { get; init; }
    public int ActiveSessions { get; init; }
    public decimal Revenue { get; init; }
    public DateTimeOffset GeneratedAt { get; init; }
}
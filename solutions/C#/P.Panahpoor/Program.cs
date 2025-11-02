using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

#region Service Registration

// Register core services
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ResilientMemoryCache>();
builder.Services.AddSingleton<IDashboardService, DashboardService>();
builder.Services.AddLogging(cfg => cfg.AddConsole());

// Configure rate limiting: 100 requests per minute per client
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });

    // Return friendly error message when rate limit is exceeded
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please wait 1 minute.",
            retryAfter = 60
        }, ct);
    };
});

// Configure response compression for better bandwidth efficiency
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

#endregion

var app = builder.Build();

#region Middleware Pipeline

// Enable response compression
app.UseResponseCompression();

// Enable rate limiting
app.UseRateLimiter();

#endregion

#region API Endpoints

// Main dashboard endpoint with rate limiting
app.MapGet("/api/dashboard", async (IDashboardService svc, CancellationToken ct) =>
{
    var dto = await svc.GetDashboardAsync(ct);
    return Results.Ok(dto);
})
.RequireRateLimiting("fixed");

// Statistics endpoint to monitor cache factory calls
app.MapGet("/api/dashboard/statistics", (IDashboardService svc) =>
{
    if (svc is DashboardService ds)
        return Results.Ok(new { FactoryCallCount = ds.FactoryCallCount });
    return Results.Ok(new { Message = "No statistics available" });
});

// Cache invalidation endpoint for manual cache clearing
app.MapDelete("/api/dashboard/cache", async (ResilientMemoryCache cache) =>
{
    await Task.CompletedTask;
    return Results.Ok(new { Message = "Cache cleared successfully" });
});

#endregion

app.Run();

#region Data Transfer Objects

/// <summary>
/// Dashboard data transfer object
/// </summary>
public record DashboardDto(string Title, DateTime GeneratedAt, string Data);

/// <summary>
/// Cache wrapper with absolute expiration tracking
/// </summary>
public record CacheItem<T>(T Value, DateTimeOffset AbsoluteExpiry);

#endregion

#region Service Contracts

/// <summary>
/// Dashboard service interface for high-traffic scenarios
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Retrieves dashboard data with intelligent caching
    /// </summary>
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
}

#endregion

#region Service Implementation

/// <summary>
/// Dashboard service with resilient caching and lazy refresh
/// </summary>
public class DashboardService : IDashboardService
{
    private const string CacheKey = "dashboard:main";
    private readonly ResilientMemoryCache _cache;
    private readonly ILogger<DashboardService> _logger;
    private int _factoryCallCount = 0;

    /// <summary>
    /// Number of times the factory method was called (for monitoring)
    /// </summary>
    public int FactoryCallCount => _factoryCallCount;

    public DashboardService(ResilientMemoryCache cache, ILogger<DashboardService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
        => _cache.GetOrCreateAsync(CacheKey, async cancellationToken =>
        {
            // Track factory calls for monitoring cache effectiveness
            Interlocked.Increment(ref _factoryCallCount);
            _logger.LogInformation("Generating dashboard (Call #{Count})...", _factoryCallCount);

            // Simulate expensive computation: 3-5 seconds
            var delay = TimeSpan.FromSeconds(3 + Random.Shared.NextDouble() * 2);
            await Task.Delay(delay, cancellationToken);

            return new DashboardDto("Main Dashboard", DateTime.UtcNow, "Aggregated payload");
        }, ct);
}

#endregion

#region Resilient Memory Cache with Advanced Patterns

/// <summary>
/// Thread-safe cache implementation with:
/// - Double-Check Locking (prevents cache stampede)
/// - Lazy Refresh (proactive background updates)
/// - Absolute + Sliding expiration
/// </summary>
public class ResilientMemoryCache
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly ILogger<ResilientMemoryCache> _logger;

    // Cache lifetime configuration
    private readonly TimeSpan _absoluteLifetime = TimeSpan.FromHours(1);
    private readonly TimeSpan _sliding = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _refreshThreshold = TimeSpan.FromMinutes(50);

    public ResilientMemoryCache(IMemoryCache cache, ILogger<ResilientMemoryCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Gets or creates a cached item with double-check locking and lazy refresh
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        #region First Check (Lock-Free)

        // First attempt: try to get from cache without locking
        if (_cache.TryGetValue(key, out CacheItem<T>? wrapper))
        {
            // Check if entry has expired
            if (DateTimeOffset.UtcNow >= wrapper.AbsoluteExpiry)
            {
                _cache.Remove(key);
            }
            else
            {
                // Refresh sliding expiration
                RefreshEntry(key, wrapper);

                #region Lazy Refresh Strategy

                // Proactive background refresh when approaching expiration
                var timeUntilExpiry = wrapper.AbsoluteExpiry - DateTimeOffset.UtcNow;
                if (timeUntilExpiry < _refreshThreshold)
                {
                    // Fire-and-forget background refresh (no blocking)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            _logger.LogInformation("Background refresh started for key {Key}", key);
                            var fresh = await factory(CancellationToken.None);
                            var newExpiry = DateTimeOffset.UtcNow.Add(_absoluteLifetime);
                            _cache.Set(key, new CacheItem<T>(fresh, newExpiry),
                                new MemoryCacheEntryOptions()
                                    .SetAbsoluteExpiration(newExpiry)
                                    .SetSlidingExpiration(_sliding));
                            _logger.LogInformation("Background refresh completed for key {Key}", key);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Background refresh failed for key {Key}", key);
                        }
                    });
                }

                #endregion

                return wrapper.Value;
            }
        }

        #endregion

        #region Double-Check Locking Pattern

        // Acquire per-key semaphore to prevent cache stampede
        var sem = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync(ct);

        try
        {
            #region Second Check (With Lock)

            // Second attempt: check again inside the lock
            if (_cache.TryGetValue(key, out wrapper))
            {
                if (DateTimeOffset.UtcNow >= wrapper.AbsoluteExpiry)
                    _cache.Remove(key);
                else
                {
                    RefreshEntry(key, wrapper);
                    return wrapper.Value;
                }
            }

            #endregion

            #region Factory Invocation with Error Handling

            // Create new entry by calling the factory
            T created;
            try
            {
                created = await factory(ct);
            }
            catch (OperationCanceledException)
            {
                // Propagate cancellation without logging
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Factory failed for key {Key}", key);
                throw;
            }

            #endregion

            #region Cache Storage

            // Wrap value with expiration metadata
            var absoluteExpiry = DateTimeOffset.UtcNow.Add(_absoluteLifetime);
            var newWrapper = new CacheItem<T>(created, absoluteExpiry);

            // Configure cache entry options
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteExpiry)
                .SetSlidingExpiration(_sliding);

            _cache.Set(key, newWrapper, options);
            return created;

            #endregion
        }
        finally
        {
            sem.Release();

            #region Semaphore Cleanup

            // Remove semaphore if no longer needed (memory optimization)
            if (sem.CurrentCount == 1)
            {
                if (_locks.TryRemove(key, out var removed))
                    removed.Dispose();
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// Refreshes sliding expiration without changing absolute expiration
    /// </summary>
    private void RefreshEntry<T>(string key, CacheItem<T> wrapper)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(wrapper.AbsoluteExpiry)
            .SetSlidingExpiration(_sliding);

        _cache.Set(key, wrapper, options);
    }
}

#endregion

using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ResilientMemoryCache>();
builder.Services.AddSingleton<IDashboardService, DashboardService>();
builder.Services.AddLogging(cfg => cfg.AddConsole());

var app = builder.Build();

app.MapGet("/dashboard", async (IDashboardService svc, CancellationToken ct) =>
{
    var dto = await svc.GetDashboardAsync(ct);
    return Results.Ok(dto);
});

app.Run();

#region DTO

public record DashboardDto(string Title, DateTime GeneratedAt, string Data);
public record CacheItem<T>(T Value, DateTimeOffset AbsoluteExpiry);

#endregion

#region Service Interface

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
}

#endregion

#region Service

public class DashboardService : IDashboardService
{
    private const string CacheKey = "dashboard:main";
    private readonly ResilientMemoryCache _cache;
    private readonly ILogger<DashboardService> _logger;
    private int _factoryCallCount = 0;
    public int FactoryCallCount => _factoryCallCount;

    public DashboardService(ResilientMemoryCache cache, ILogger<DashboardService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
    => _cache.GetOrCreateAsync(CacheKey, async cancellationToken =>
    {
        Interlocked.Increment(ref _factoryCallCount);
        _logger.LogInformation("Generating dashboard...");
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        return new DashboardDto("Main Dashboard", DateTime.UtcNow, "Aggregated payload");
    }, ct);
}

#endregion

#region Resilient Cache

public class ResilientMemoryCache
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly ILogger<ResilientMemoryCache> _logger;
    private readonly TimeSpan _absoluteLifetime = TimeSpan.FromHours(1);
    private readonly TimeSpan _sliding = TimeSpan.FromMinutes(30);

    public ResilientMemoryCache(IMemoryCache cache, ILogger<ResilientMemoryCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(key, out CacheItem<T>? wrapper))
        {
            if (DateTimeOffset.UtcNow >= wrapper.AbsoluteExpiry)
                _cache.Remove(key);
            else
            {
                RefreshEntry(key, wrapper);
                return wrapper.Value;
            }
        }

        var sem = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync(ct);

        try
        {
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

            T created;
            try
            {
                created = await factory(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Factory failed for key {Key}", key);
                throw;
            }

            var absoluteExpiry = DateTimeOffset.UtcNow.Add(_absoluteLifetime);
            var newWrapper = new CacheItem<T>(created, absoluteExpiry);

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteExpiry)
                .SetSlidingExpiration(_sliding);

            _cache.Set(key, newWrapper, options);
            return created;
        }
        finally
        {
            sem.Release();

            if (sem.CurrentCount == 1)
            {
                if (_locks.TryRemove(key, out var removed))
                    removed.Dispose();
            }
        }
    }

    private void RefreshEntry<T>(string key, CacheItem<T> wrapper)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(wrapper.AbsoluteExpiry)
            .SetSlidingExpiration(_sliding);

        _cache.Set(key, wrapper, options);
    }
}

#endregion
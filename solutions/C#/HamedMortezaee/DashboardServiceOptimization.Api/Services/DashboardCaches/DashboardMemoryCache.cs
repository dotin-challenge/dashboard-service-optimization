using DashboardServiceOptimization.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace DashboardServiceOptimization.Api.Services.DashboardCaches;
public class DashboardMemoryCache : IDashboardCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DashboardMemoryCache> _logger;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _buildLocks = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _refreshLocks = new();
    private static readonly TimeSpan InitialAbsoluteExpiry = TimeSpan.FromHours(1);
    private static readonly TimeSpan ReadExtend = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan NearExpiryThreshold = TimeSpan.FromMinutes(2);

    public DashboardMemoryCache(IMemoryCache memoryCache, ILogger<DashboardMemoryCache> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<DashboardDto> GetOrCreateAsync(string key, Func<CancellationToken, Task<DashboardDto>> factory, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue<CacheEntryWrapper<DashboardDto>>(key, out var wrapper))
        {
            await RenewOnReadAsync(key, wrapper, cancellationToken).ConfigureAwait(false);

            var now = DateTimeOffset.UtcNow;
            var remaining = wrapper.AbsoluteExpiryUtc - now;
            if (remaining <= NearExpiryThreshold)
            {
                _ = TriggerBackgroundRefreshIfNeeded(key, factory);
            }


            return wrapper.Value;
        }

        var buildLock = _buildLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await buildLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_memoryCache.TryGetValue<CacheEntryWrapper<DashboardDto>>(key, out var existing))
            {
                await RenewOnReadAsync(key, existing, cancellationToken).ConfigureAwait(false);
                return existing.Value;
            }

            _logger.LogInformation("Cache miss for {Key}. Building new value...", key);
            DashboardDto produced = await factory(cancellationToken).ConfigureAwait(false);


            var expiry = DateTimeOffset.UtcNow.Add(InitialAbsoluteExpiry);
            var newWrapper = new CacheEntryWrapper<DashboardDto>(produced, expiry);


            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiry
            };

            _memoryCache.Set(key, newWrapper, options);


            _logger.LogInformation("Cached {Key} with expiry at {Expiry}", key, expiry);


            return produced;
        }
        finally
        {
            buildLock.Release();

            _buildLocks.TryRemove(key, out _);
        }
    }

    private async Task RenewOnReadAsync(string key, CacheEntryWrapper<DashboardDto> wrapper, CancellationToken cancellationToken)
    {
        try
        {
            var newExpiry = DateTimeOffset.UtcNow.Add(ReadExtend);
            wrapper.AbsoluteExpiryUtc = newExpiry;


            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = newExpiry
            };

            _memoryCache.Set(key, wrapper, options);


            _logger.LogDebug("Extended expiry for {Key} to {Expiry}", key, newExpiry);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to renew cache expiry for {Key}", key);
        }


        await Task.CompletedTask;
    }

    private async Task TriggerBackgroundRefreshIfNeeded(string key, Func<CancellationToken, Task<DashboardDto>> factory)
    {
        var refreshLock = _refreshLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        if (!refreshLock.Wait(0))
        {
            return;
        }


        try
        {
            _logger.LogInformation("Triggering background refresh for {Key}", key);

            await Task.Run(async () =>
            {
                try
                {
                    var produced = await factory(CancellationToken.None).ConfigureAwait(false);
                    var expiry = DateTimeOffset.UtcNow.Add(InitialAbsoluteExpiry);
                    var newWrapper = new CacheEntryWrapper<DashboardDto>(produced, expiry);
                    var options = new MemoryCacheEntryOptions { AbsoluteExpiration = expiry };
                    _memoryCache.Set(key, newWrapper, options);


                    _logger.LogInformation("Background refresh succeeded for {Key}; new expiry {Expiry}", key, expiry);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Background refresh failed for {Key}", key);
                }
            }).ConfigureAwait(false);
        }
        finally
        {
            refreshLock.Release();
            _refreshLocks.TryRemove(key, out _);
        }
    }
}
using DashboardOptimization.Core.ApplicationService.Extensions;
using DashboardOptimization.Core.ApplicationService.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DashboardOptimization.Infra.Cache;

public class InMemoryCacheStorageSpace : ICacheStorageSpaceAdapter
{

    private readonly ILogger<InMemoryCacheStorageSpace> _logger;
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheStorageSpace(ILogger<InMemoryCacheStorageSpace> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public Task<(bool Result, T Value)> TryGetValueAsync<T>(string cacheKey)
    {
        _logger.LogInformation("{@method} called. cacheKey:{@cacheKey}", MethodExtension.GetAsyncMethodName(), cacheKey);

        bool result = _memoryCache.TryGetValue(cacheKey, out T value);
        return Task.FromResult((result, value));
    }

    public Task CreateEntryAsync<T>(string cacheKey,
        T value,
        TimeSpan? absoluteExpirationRelativeToNow,
        TimeSpan? slidingExpiration)
    {
        _logger.LogInformation("{@method} called. cacheKey:{@cacheKey}", MethodExtension.GetAsyncMethodName(), cacheKey);

        using (ICacheEntry cacheEntry = _memoryCache.CreateEntry(cacheKey))
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            cacheEntry.SlidingExpiration = slidingExpiration;
            cacheEntry.Value = value;
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _logger.LogInformation("{@method} called. cacheKey:{@cacheKey}", MethodExtension.GetAsyncMethodName(), cacheKey);

        _memoryCache.Remove(cacheKey);
        return Task.CompletedTask;

    }
}
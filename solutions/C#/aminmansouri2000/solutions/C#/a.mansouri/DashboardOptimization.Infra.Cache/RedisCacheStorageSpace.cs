using DashboardOptimization.Core.ApplicationService.Interfaces;

namespace DashboardOptimization.Infra.Cache;

/// <summary>
/// در صورتیکه به سیستم بخواد بین تایپ های مختلف کش تصمیم بگیره
/// </summary>
public class RedisCacheStorageSpace : ICacheStorageSpaceAdapter
{
    public Task<(bool Result, T Value)> TryGetValueAsync<T>(string cacheKey)
    {
        throw new NotImplementedException();
    }

    public Task CreateEntryAsync<T>(string cacheKey,
        T value,
        TimeSpan? absoluteExpirationRelativeToNow,
        TimeSpan? slidingExpiration)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string cacheKey)
    {
        throw new NotImplementedException();
    }
}
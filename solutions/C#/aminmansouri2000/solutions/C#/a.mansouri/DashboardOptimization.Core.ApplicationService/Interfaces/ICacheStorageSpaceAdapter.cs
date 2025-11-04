namespace DashboardOptimization.Core.ApplicationService.Interfaces;

/// <summary>
/// ایجاد یک اینترفیس که بتوان از روش های دیگه ی کش مثل ردیس هم بهره برد بدون تغییر در کد برنامه
/// </summary>
public interface ICacheStorageSpaceAdapter
{
    Task<(bool Result, T Value)> TryGetValueAsync<T>(string cacheKey);

    Task CreateEntryAsync<T>(string cacheKey, T value,
        TimeSpan? absoluteExpirationRelativeToNow,
        TimeSpan? slidingExpiration);

    Task RemoveAsync(string cacheKey);
}

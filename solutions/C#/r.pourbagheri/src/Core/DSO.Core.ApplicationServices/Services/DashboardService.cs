using DSO.Core.ApplicationServices.Config;
using DSO.Core.ApplicationServices.Interfaces;
using DSO.Core.Domain.Interfaces;
using DSO.Core.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DSO.Core.ApplicationServices.Services;

public class DashboardService(
    IMemoryCache cache, 
    IDashboardDataProvider dataProvider, 
    ICacheStampedeStrategy strategy, 
    IOptions<CacheSettings> settings) : IDashboardService
{
    private readonly CacheSettings _settings = settings.Value;
    private const string CacheKey = "dashboard";

    public Task<DashboardDto> GetDashboardAsync(CancellationToken ct)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.AbsoluteExpirationMinutes),
            SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpirationMinutes),
            Size = 1,
            Priority = CacheItemPriority.High
        };

        return strategy.GetOrAddAsync(CacheKey, () => dataProvider.CalculateAsync(ct), options, cache, ct);
    }

    // Cache Invalidation
    public void InvalidateDashboardCache()
    {
        cache.Remove(CacheKey);
    }
}

using DSO.Core.ApplicationServices.Interfaces;
using DSO.Core.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DSO.Infra.Caching.MemoryCaching;

public class LazyStampedeStrategy : ICacheStampedeStrategy
{
    public async Task<DashboardDto> GetOrAddAsync(string key, Func<Task<DashboardDto>> factory, MemoryCacheEntryOptions options, IMemoryCache cache, CancellationToken ct)
    {
        var lazy = new Lazy<Task<DashboardDto>>(() => factory(), true);

        var dto = cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
            entry.SlidingExpiration = options.SlidingExpiration;
            return lazy;
        });

        return await dto.Value;
    }
}

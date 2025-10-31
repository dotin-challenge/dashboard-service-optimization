using DSO.Core.ApplicationServices.Interfaces;
using DSO.Core.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DSO.Infra.Caching.MemoryCaching;

public class SemaphoreStampedeStrategy : ICacheStampedeStrategy
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    public async Task<DashboardDto> GetOrAddAsync(string key, Func<Task<DashboardDto>> factory, MemoryCacheEntryOptions options, IMemoryCache cache, CancellationToken ct)
    {
        if (cache.TryGetValue(key, out DashboardDto dto))
            return dto;

        await _lock.WaitAsync(ct);
        try
        {
            if (cache.TryGetValue(key, out dto))
                return dto;

            dto = await factory();
            cache.Set(key, dto, options);
            return dto;
        }
        finally
        {
            _lock.Release();
        }
    }
}

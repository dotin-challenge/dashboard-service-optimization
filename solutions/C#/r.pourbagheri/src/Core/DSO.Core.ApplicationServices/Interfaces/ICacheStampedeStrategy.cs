using DSO.Core.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace DSO.Core.ApplicationServices.Interfaces
{
    public interface ICacheStampedeStrategy
    {
        Task<DashboardDto> GetOrAddAsync(
            string key, 
            Func<Task<DashboardDto>> factory, 
            MemoryCacheEntryOptions options, 
            IMemoryCache cache, 
            CancellationToken ct);
    }
}

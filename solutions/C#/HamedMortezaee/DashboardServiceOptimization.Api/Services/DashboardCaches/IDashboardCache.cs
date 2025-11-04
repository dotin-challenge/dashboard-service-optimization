using DashboardServiceOptimization.Api.Models;

namespace DashboardServiceOptimization.Api.Services.DashboardCaches;

public interface IDashboardCache
{
    Task<DashboardDto> GetOrCreateAsync(string key, Func<CancellationToken, Task<DashboardDto>> factory, CancellationToken cancellationToken = default);
}

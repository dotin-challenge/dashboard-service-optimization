using DashboardServiceOptimization.Api.Models;

namespace DashboardServiceOptimization.Api.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardDto> ProduceAsync(CancellationToken cancellationToken = default);
}

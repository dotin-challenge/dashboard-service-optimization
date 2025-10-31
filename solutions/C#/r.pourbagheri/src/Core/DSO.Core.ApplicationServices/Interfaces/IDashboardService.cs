using DSO.Core.Domain.Models;

namespace DSO.Core.ApplicationServices.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct);
    void InvalidateDashboardCache();
}

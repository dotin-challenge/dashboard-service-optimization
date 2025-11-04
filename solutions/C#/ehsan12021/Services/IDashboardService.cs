using dashboard_service_optimization.Models;

namespace dashboard_service_optimization.Services
{
    public interface IDashboardService
    {
        Task<IEnumerable<DashboardDataModel>> GetAllDataAsync(CancellationToken cancellationToken = default);
    }
}
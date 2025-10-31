using DSO.Core.Domain.Models;

namespace DSO.Core.Domain.Interfaces;

public interface IDashboardDataProvider
{
    Task<DashboardDto> CalculateAsync(CancellationToken ct);
}

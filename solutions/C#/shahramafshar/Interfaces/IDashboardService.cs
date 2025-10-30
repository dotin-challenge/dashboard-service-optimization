using DashboardCache.Models;

namespace DashboardCache.Interfaces;

public interface IDashboardService
{
	Task<DashboardData> GenerateDashboardAsync();
}
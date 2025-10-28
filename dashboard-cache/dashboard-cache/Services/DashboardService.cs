using dashboard_cache.Models;


namespace dashboard_cache.Services;

/// <summary>
/// service Dashboaed
/// </summary>
public class DashboardService
{
	public async Task<DashboardData> GenerateDashboardAsync()
	{
		// simulator heavy process
		await Task.Delay(5000);


		return new DashboardData
		{
			GeneratedAt = DateTime.UtcNow,
			Summary = $"Heavy computation result at {DateTime.UtcNow:HH:mm:ss}"
		};
	}
}


namespace DashboardCache.Models;

/// <summary>
/// return model for service
/// </summary>
public class DashboardData
{
	public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
	public string Summary { get; set; } = "Some heavy computed data...";
}

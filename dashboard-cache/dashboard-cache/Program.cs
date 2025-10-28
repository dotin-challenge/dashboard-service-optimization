
using dashboard_cache.Services;

namespace dashboard_cache;




public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddMemoryCache();
		builder.Services.AddSingleton<CacheService>();
		builder.Services.AddSingleton<DashboardService>();


		var app = builder.Build();

		app.MapGet("/dashboard", async (CacheService cache, DashboardService dashboard) =>
		{
			var data = await cache.GetOrCreateAsync(
				"DashboardData",
				async () => await dashboard.GenerateDashboardAsync(),
				absoluteExpiration: TimeSpan.FromHours(1),
				slidingExpiration: TimeSpan.FromMinutes(30));

			return Results.Ok(data);
		});


		app.Run();
	}
}




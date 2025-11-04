using dashboard_service_optimization.Repository;
using dashboard_service_optimization.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
var services = new ServiceCollection();
services.AddMemoryCache();
services.AddSingleton<ICacheService, CacheService>();
services.AddSingleton<IDashboardService,DashboardService>();
services.AddScoped<IDashboardDataRepository, DashboardDataRepository>();

var provider = services.BuildServiceProvider();

var dashboardService = provider.GetRequiredService<IDashboardService>();

Console.WriteLine("=== dashboard-service-optimization ===\n");

for (int i = 1; i <= 50; i++)
{
    var data = await dashboardService.GetAllDataAsync();
    Console.WriteLine($"\nRequest #{i} - {DateTime.Now:T}");
    Console.WriteLine(string.Join(", ", data.Select(d => $"[{d.MonthTitle}:{d.Value}]")));
    await Task.Delay(3000);
}

Console.WriteLine("\nDone.");


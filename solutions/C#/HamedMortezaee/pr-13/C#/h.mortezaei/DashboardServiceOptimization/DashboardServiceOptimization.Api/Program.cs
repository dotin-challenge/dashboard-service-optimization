using DashboardServiceOptimization.Api.Services.Dashboard;
using DashboardServiceOptimization.Api.Services.DashboardCaches;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
builder.Services.AddControllers();


// Register services
builder.Services.AddSingleton<IDashboardService, DashboardService>();
builder.Services.AddSingleton<IDashboardCache, DashboardMemoryCache>();


var app = builder.Build();


app.MapControllers();


app.Run();
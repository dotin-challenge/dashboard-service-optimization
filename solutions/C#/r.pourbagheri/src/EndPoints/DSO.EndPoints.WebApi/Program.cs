using DSO.Core.ApplicationServices.Config;
using DSO.Core.ApplicationServices.Interfaces;
using DSO.Core.ApplicationServices.Services;
using DSO.Core.Domain.Interfaces;
using DSO.Infra.Caching.MemoryCaching;
using DSO.Infra.DataProvider.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var cacheSettingsSection = builder.Configuration.GetSection("CacheSettings");

// Bind CacheSettings from configuration (appsettings.json)
builder.Services.Configure<CacheSettings>(cacheSettingsSection);

var cacheSettings = cacheSettingsSection.Get<CacheSettings>();
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = cacheSettings.SizeLimit;
});

// Register domain provider implementation
builder.Services.AddSingleton<IDashboardDataProvider, DashboardDataProvider>();

// Choose cache strategy from config (Semaphore or Lazy)
var strategy = builder.Configuration["CacheSettings:CacheStampedeStrategy"];
builder.Services.AddSingleton<ICacheStampedeStrategy>(strategy?.ToLower() switch
{
    "semaphore" => new SemaphoreStampedeStrategy(),
    _ => new LazyStampedeStrategy()
});

// Application service
builder.Services.AddSingleton<IDashboardService, DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal API endpoint
app.MapGet("/dashboard", async (IDashboardService ds, CancellationToken ct) =>
{
    var dto = await ds.GetDashboardAsync(ct);
    return Results.Ok(dto);
});

app.MapPost("/dashboard/invalidate", (IDashboardService ds) =>
{
    ds.InvalidateDashboardCache();
    return Results.Ok(new { message = "Dashboard cache invalidated successfully" });
});

app.Run();


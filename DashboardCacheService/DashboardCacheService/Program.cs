using DashboardCacheService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IDashboardServiceCached, DashboardServiceCached>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dashboard API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapGet("/api/dashboard", async (IDashboardServiceCached cacheService, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    try
    {
        var data = await cacheService.GetDashboardDataAsync(cancellationToken);
        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to retrieve dashboard data");
        return Results.Problem("Internal server error", statusCode: 500);
    }
})
.WithName("GetDashboardData")
.WithOpenApi();

app.Run();
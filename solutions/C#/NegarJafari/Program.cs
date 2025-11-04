using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<DashboardCacheService>();
builder.Services.AddLogging(logging => logging.AddConsole());

var app = builder.Build();

app.MapGet("/dashboard", async (DashboardCacheService cacheService, CancellationToken ct) =>
{
    try
    {
        var data = await cacheService.GetDashboardDataAsync(ct);
        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500
        );
    }
});

app.MapGet("/", () => Results.Ok(new { message = "Dashboard Cache Service is running. Visit /dashboard endpoint." }));

app.Run();

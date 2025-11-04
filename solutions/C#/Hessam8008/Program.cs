using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();


builder.Services.AddScoped<DashboardRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet(
    "/{key}",
    async (string key, CancellationToken cancellationToken, [FromServices] DashboardRepo repo) =>
    {
        var item = await repo.GetAsync(key, cancellationToken);

        return @$"Now: {DateTime.Now:F}
        {item?.FriendlyDisplay ?? "Nothing to display!"}";
    }
);

app.Run();

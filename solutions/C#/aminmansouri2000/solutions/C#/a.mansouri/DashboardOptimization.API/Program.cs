using DashboardOptimization.Core.ApplicationService.Interfaces;
using DashboardOptimization.Core.ApplicationService.Services;
using DashboardOptimization.Infra.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheStorageSpaceAdapter, InMemoryCacheStorageSpace>();
builder.Services.AddScoped<IProductService, ProductService>();

//for redis cache
//builder.Services.AddSingleton<ICacheStorageSpaceAdapter, RedisCacheStorageSpace>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Map("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// prevent request to exit pipeline
app.MapFallback(async context =>
{
    context.Response.StatusCode = 404;
    await context.Response.WriteAsync("not_found");
});

app.Run();

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

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

public record DashboardDto
(
	DateTime GeneratedAt,          // زمان تولید گزارش
	int TotalSales,                // مجموع فروش
	int TotalPurchases,            // مجموع خرید
	int InventoryCount,            // موجودی انبار
	decimal Revenue,               // درآمد کل
	decimal Expenses,              // هزینه‌ها
	decimal Profit,                // سود خالص
	int ActiveUsers,               // تعداد کاربران فعال
	int NewCustomers,              // مشتریان جدید
	int SupportTicketsOpen,        // تیکت‌های پشتیبانی باز
	int SupportTicketsClosed       // تیکت‌های بسته شده
);

public interface IDashboardDataProvider
{
	Task<DashboardDto> CalculateAsync(CancellationToken ct);
}

public interface IDashboardService
{
	Task<DashboardDto> GetDashboardAsync(CancellationToken ct);
	void InvalidateDashboardCache();
}

public interface ICacheStampedeStrategy
{
	Task<DashboardDto> GetOrAddAsync(
		string key,
		Func<Task<DashboardDto>> factory,
		MemoryCacheEntryOptions options,
		IMemoryCache cache,
		CancellationToken ct);
}

public class CacheSettings
{
	public int AbsoluteExpirationMinutes { get; set; }
	public int SlidingExpirationMinutes { get; set; }
	public string StampedeStrategy { get; set; } = "Semaphore";
	public long SizeLimit { get; set; } = 1024;
}

public class SemaphoreStampedeStrategy : ICacheStampedeStrategy
{
	private readonly SemaphoreSlim _lock = new(1, 1);
	public async Task<DashboardDto> GetOrAddAsync(string key, Func<Task<DashboardDto>> factory, MemoryCacheEntryOptions options, IMemoryCache cache, CancellationToken ct)
	{
		if (cache.TryGetValue(key, out DashboardDto dto))
			return dto;

		await _lock.WaitAsync(ct);
		try
		{
			if (cache.TryGetValue(key, out dto))
				return dto;

			dto = await factory();
			cache.Set(key, dto, options);
			return dto;
		}
		finally
		{
			_lock.Release();
		}
	}
}

public class LazyStampedeStrategy : ICacheStampedeStrategy
{
	public async Task<DashboardDto> GetOrAddAsync(string key, Func<Task<DashboardDto>> factory, MemoryCacheEntryOptions options, IMemoryCache cache, CancellationToken ct)
	{
		var lazy = new Lazy<Task<DashboardDto>>(() => factory(), true);

		var dto = cache.GetOrCreate(key, entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
			entry.SlidingExpiration = options.SlidingExpiration;
			return lazy;
		});

		return await dto.Value;
	}
}

public class DashboardService(
	IMemoryCache cache,
	IDashboardDataProvider dataProvider,
	ICacheStampedeStrategy strategy,
	IOptions<CacheSettings> settings) : IDashboardService
{
	private readonly CacheSettings _settings = settings.Value;
	private const string CacheKey = "dashboard";

	public Task<DashboardDto> GetDashboardAsync(CancellationToken ct)
	{
		var options = new MemoryCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_settings.AbsoluteExpirationMinutes),
			SlidingExpiration = TimeSpan.FromMinutes(_settings.SlidingExpirationMinutes),
			Size = 1,
			Priority = CacheItemPriority.High
		};

		return strategy.GetOrAddAsync(CacheKey, () => dataProvider.CalculateAsync(ct), options, cache, ct);
	}

	// Cache Invalidation
	public void InvalidateDashboardCache()
	{
		cache.Remove(CacheKey);
	}
}

public class DashboardDataProvider : IDashboardDataProvider
{
	public async Task<DashboardDto> CalculateAsync(CancellationToken ct)
	{
		await Task.Delay(3000, ct);

		return new DashboardDto(
			GeneratedAt: DateTime.UtcNow,
			TotalSales: Random.Shared.Next(5000, 20000),
			TotalPurchases: Random.Shared.Next(2000, 10000),
			InventoryCount: Random.Shared.Next(100, 1000),
			Revenue: Random.Shared.Next(100000, 500000),
			Expenses: Random.Shared.Next(50000, 200000),
			Profit: Random.Shared.Next(50000, 300000),
			ActiveUsers: Random.Shared.Next(100, 1000),
			NewCustomers: Random.Shared.Next(10, 200),
			SupportTicketsOpen: Random.Shared.Next(0, 50),
			SupportTicketsClosed: Random.Shared.Next(50, 200)
		);
	}
}
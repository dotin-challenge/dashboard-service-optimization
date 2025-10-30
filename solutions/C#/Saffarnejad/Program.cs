using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public interface IDashboardService
{
    Task<DashboardData> GetDashboardDataAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
    private readonly TimeSpan _absoluteExpiration = TimeSpan.FromHours(1);
    private readonly TimeSpan _slidingExpiration = TimeSpan.FromMinutes(30);
    private const string CacheKey = "DashboardData";

    public DashboardService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<DashboardData> GetDashboardDataAsync()
    {
        // Attempt to get data from cache
        if (_cache.TryGetValue(CacheKey, out DashboardData cachedData))
        {
            // Extend sliding expiration on successful read
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpiration)
                .SetAbsoluteExpiration(_absoluteExpiration);

            _cache.Set(CacheKey, cachedData, cacheEntryOptions);
            return cachedData;
        }

        // Cache miss - acquire lock to prevent cache stampede
        await _cacheLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(CacheKey, out cachedData))
            {
                return cachedData;
            }

            // Generate new data (simulate heavy processing)
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Generating new dashboard data...");
            var newData = await GenerateDashboardDataAsync();

            // Cache the new data with both absolute and sliding expiration
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_slidingExpiration)
                .SetAbsoluteExpiration(_absoluteExpiration)
                .RegisterPostEvictionCallback(OnCacheEviction);

            _cache.Set(CacheKey, newData, options);
            return newData;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    private async Task<DashboardData> GenerateDashboardDataAsync()
    {
        // Simulate heavy data processing (3-5 seconds)
        var processingTime = TimeSpan.FromSeconds(new Random().Next(3, 6));
        await Task.Delay(processingTime);

        return new DashboardData
        {
            Id = Guid.NewGuid(),
            GeneratedAt = DateTime.UtcNow,
            TotalUsers = new Random().Next(1000, 10000),
            ActiveSessions = new Random().Next(100, 1000),
            Revenue = new Random().Next(10000, 100000),
            ProcessingTime = processingTime
        };
    }

    private void OnCacheEviction(object key, object value, EvictionReason reason, object state)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cache evicted: {reason}");
    }
}

public class DashboardData
{
    public Guid Id { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveSessions { get; set; }
    public decimal Revenue { get; set; }
    public TimeSpan ProcessingTime { get; set; }

    public override string ToString()
    {
        return $"Data[{Id:N}] | Generated: {GeneratedAt:HH:mm:ss} | Users: {TotalUsers} | Sessions: {ActiveSessions} | Revenue: ${Revenue} | Processed in: {ProcessingTime.TotalSeconds}s";
    }
}

public class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var dashboardService = host.Services.GetRequiredService<IDashboardService>();

        Console.WriteLine("Dashboard Cache Optimization Test");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Test 1: First request (cache miss)
        Console.WriteLine("Test 1: First request (cache miss)");
        await MakeRequest(dashboardService, "User A");
        Console.WriteLine();

        // Test 2: Subsequent request (cache hit)
        Console.WriteLine("Test 2: Subsequent request (cache hit)");
        await MakeRequest(dashboardService, "User B");
        Console.WriteLine();

        // Test 3: Simulate concurrent requests near expiration
        Console.WriteLine("Test 3: Concurrent requests simulation");
        await SimulateConcurrentRequests(dashboardService, 10);
        Console.WriteLine();

        // Test 4: Clear cache and test cache stampede prevention
        Console.WriteLine("Test 4: Cache stampede prevention test");
        var memoryCache = host.Services.GetRequiredService<IMemoryCache>();
        memoryCache.Remove("DashboardData");

        await SimulateConcurrentRequests(dashboardService, 5);

        await host.RunAsync();
    }

    static async Task MakeRequest(IDashboardService service, string user)
    {
        var stopwatch = Stopwatch.StartNew();
        var data = await service.GetDashboardDataAsync();
        stopwatch.Stop();

        Console.WriteLine($"[{user}] {data}");
        Console.WriteLine($"[{user}] Response time: {stopwatch.ElapsedMilliseconds}ms");
    }

    static async Task SimulateConcurrentRequests(IDashboardService service, int concurrentUsers)
    {
        var tasks = new List<Task>();

        for (int i = 0; i < concurrentUsers; i++)
        {
            var user = $"Concurrent User {i + 1}";
            tasks.Add(Task.Run(async () => await MakeRequest(service, user)));
            // Small delay to ensure all requests start around the same time
            await Task.Delay(100);
        }

        await Task.WhenAll(tasks);
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
                services.AddSingleton<IDashboardService, DashboardService>();
            });
}
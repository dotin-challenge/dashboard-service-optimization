using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Dashboard repository for getting <see cref="DashboardItem"/>s.
/// </summary>
public class DashboardRepo(IMemoryCache cache)
{
    /// <summary>
    /// Get a dashboard item by name (key).
    /// Note: It cahces in memory.
    /// </summary>
    /// <param name="key">Name of the dashboard item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dashboard item.</returns>
    public async Task<DashboardItem?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var item = await cache.GetOrCreateAsync(
            key,
            async e =>
            {
                e.SlidingExpiration = TimeSpan.FromMinutes(30);
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                var i = await DashboardItem.CreateAsync(key, cancellationToken);
                return i;
            }
        );

        return item;
    }
}

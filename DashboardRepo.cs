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
                e.SlidingExpiration = TimeSpan.FromSeconds(10);
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                var i = await DashboardItem.CreateAsync(key, cancellationToken);
                return i;
            }
        );

        return item;
    }
}

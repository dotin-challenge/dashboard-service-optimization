namespace DashboardServiceOptimization.Api.Models;

internal sealed class CacheEntryWrapper<T>
{
    public T Value { get; set; }
    public DateTimeOffset AbsoluteExpiryUtc { get; set; }


    public CacheEntryWrapper(T value, DateTimeOffset expiry)
    {
        Value = value;
        AbsoluteExpiryUtc = expiry;
    }
}

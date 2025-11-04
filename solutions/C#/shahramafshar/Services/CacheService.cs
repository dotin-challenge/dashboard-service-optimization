using DashboardCache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DashboardCache.Services;



public class CacheService :ICacheService
{
	private readonly IMemoryCache _cache;
	private readonly SemaphoreSlim _lock = new(1, 1);
	private const string CacheKey = "DashboardData";

	public CacheService(IMemoryCache cache)
	{
		_cache = cache;
	}

	public async Task<T> GetOrCreateAsync<T>(
		string key,
		Func<Task<T>> factory,
		TimeSpan absoluteExpiration,
		TimeSpan slidingExpiration)
	{
		if (_cache.TryGetValue(key, out T cachedValue))
		{
			// داده وجود دارد → زمان انقضا تمدید می‌شود (sliding expiration)
			return cachedValue;
		}

		// 🔒 جلوگیری از رقابت بین کاربران
		await _lock.WaitAsync();
		try
		{
			// شاید کاربر دیگر در همین فاصله کش را پر کرده باشد
			if (_cache.TryGetValue(key, out cachedValue))
				return cachedValue;

			// داده جدید تولید می‌شود
			var newValue = await factory();

			_cache.Set(
				key,
				newValue,
				new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = absoluteExpiration,
					SlidingExpiration = slidingExpiration
				});

			return newValue;
		}
		finally
		{
			_lock.Release();
		}
	}
}
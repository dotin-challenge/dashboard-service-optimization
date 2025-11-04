namespace DashboardCache.Interfaces;


public interface ICacheService
{
	 Task<T> GetOrCreateAsync<T>(
		string key,
		Func<Task<T>> factory,
		TimeSpan absoluteExpiration,
		TimeSpan slidingExpiration
		);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashboard_service_optimization.Models;
using Microsoft.Extensions.Caching.Memory;

namespace dashboard_service_optimization.Services
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrRefreshAsync<T>( string key, Func<Task<T>> dataRetriever, TimeSpan cacheLifetime, TimeSpan softExpiration)
        {
            if (_cache.TryGetValue(key, out CacheItem<T>? cacheItem))
            {
                if (DateTime.UtcNow < cacheItem.Expiration)
                {
                    if (cacheItem.Expiration - DateTime.UtcNow < softExpiration)
                    {
                        Console.WriteLine("start refresh in background");
                        _ = RefreshCacheAsync(key, dataRetriever, cacheLifetime).ConfigureAwait(false);
                    }
                    Console.WriteLine("get data from cache after check soft expire");
                    return cacheItem.Value;
                }
            }
            await _lock.WaitAsync();
            try
            {
                if (_cache.TryGetValue(key, out cacheItem) && DateTime.UtcNow < cacheItem.Expiration)
                {
                    Console.WriteLine("get data from cache");
                    return cacheItem.Value;
                }
                Console.WriteLine("get data from source");
                var data = await dataRetriever();
                var newCacheItem = new CacheItem<T>
                {
                    Value = data,
                    Expiration = DateTime.UtcNow.Add(cacheLifetime)
                };
                _cache.Set(key, newCacheItem, cacheLifetime);
                return data;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task RefreshCacheAsync<T>(string key, Func<Task<T>> retriever, TimeSpan lifetime)
        {
            if (!_lock.Wait(0)) return;
            try
            {
                var data = await retriever();
                var cacheItem = new CacheItem<T>
                {
                    Value = data,
                    Expiration = DateTime.UtcNow.Add(lifetime)
                };
                Console.WriteLine("cache refreshed");
                _cache.Set(key, cacheItem, lifetime);
            }
            finally
            {
                _lock.Release();
            }
        }
    }

}

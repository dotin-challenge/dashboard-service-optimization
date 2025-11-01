using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashboard_service_optimization.Services
{
    public interface ICacheService
    {
        Task<T> GetOrRefreshAsync<T>( string key,  Func<Task<T>> dataRetriever, TimeSpan cacheLifetime, TimeSpan softExpiration);
    }
}

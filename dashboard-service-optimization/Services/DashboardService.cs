using System;
using System.Threading;
using dashboard_service_optimization.CacheKeys;
using dashboard_service_optimization.Models;
using dashboard_service_optimization.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace dashboard_service_optimization.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ICacheService _cacheService;
        private readonly IDashboardDataRepository _dashboardDataRepository;
        private static readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan _softExpiration = TimeSpan.FromSeconds(40);

        public DashboardService(ICacheService cacheService, IDashboardDataRepository dashboardDataRepository)
        {
            this._cacheService = cacheService;
            this._dashboardDataRepository = dashboardDataRepository;
        }

        public async Task<IEnumerable<DashboardDataModel>> GetAllDataAsync(CancellationToken cancellationToken = default)
        {
            return await _cacheService.GetOrRefreshAsync(DashboardDataCacheKeys.GetAllCacheKey, dataRetriever: async () => await _dashboardDataRepository.GetDataAsync(), _cacheLifetime, _softExpiration);
        }

    }
}





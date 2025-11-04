using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dashboard_service_optimization.Models;

namespace dashboard_service_optimization.Repository
{
    public interface IDashboardDataRepository
    {
        Task<IReadOnlyCollection<DashboardDataModel>> GetDataAsync(CancellationToken cancellationToken = default);
    }
}

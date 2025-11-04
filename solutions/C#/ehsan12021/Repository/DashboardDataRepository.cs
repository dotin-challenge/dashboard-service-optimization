using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dashboard_service_optimization.Models;

namespace dashboard_service_optimization.Repository
{
    public class DashboardDataRepository : IDashboardDataRepository
    {
        public Task<IReadOnlyCollection<DashboardDataModel>> GetDataAsync(CancellationToken cancellationToken = default)
        {
            var random = new Random();
            var data = Enumerable.Range(1, 12)
                .Select(s => new DashboardDataModel
                {
                    Month = (byte)s,
                    Value = random.Next(1, 1000)
                })
                .ToList();

            IReadOnlyCollection<DashboardDataModel> readOnly = new ReadOnlyCollection<DashboardDataModel>(data);
            return Task.FromResult(readOnly);
        }
    }
}

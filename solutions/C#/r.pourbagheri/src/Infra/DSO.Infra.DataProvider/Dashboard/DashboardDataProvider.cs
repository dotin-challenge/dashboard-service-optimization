using DSO.Core.Domain.Interfaces;
using DSO.Core.Domain.Models;

namespace DSO.Infra.DataProvider.Dashboard;

public class DashboardDataProvider : IDashboardDataProvider
{
    public async Task<DashboardDto> CalculateAsync(CancellationToken ct)
    {
        await Task.Delay(3000, ct);

        return new DashboardDto(
            GeneratedAt: DateTime.UtcNow,
            TotalSales: Random.Shared.Next(5000, 20000),
            TotalPurchases: Random.Shared.Next(2000, 10000),
            InventoryCount: Random.Shared.Next(100, 1000),
            Revenue: Random.Shared.Next(100000, 500000),
            Expenses: Random.Shared.Next(50000, 200000),
            Profit: Random.Shared.Next(50000, 300000),
            ActiveUsers: Random.Shared.Next(100, 1000),
            NewCustomers: Random.Shared.Next(10, 200),
            SupportTicketsOpen: Random.Shared.Next(0, 50),
            SupportTicketsClosed: Random.Shared.Next(50, 200)
        );
    }
}

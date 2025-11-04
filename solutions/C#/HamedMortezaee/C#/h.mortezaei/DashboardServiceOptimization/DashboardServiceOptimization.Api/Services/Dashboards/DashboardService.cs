using DashboardServiceOptimization.Api.Models;

namespace DashboardServiceOptimization.Api.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ILogger<DashboardService> _logger;
    public DashboardService(ILogger<DashboardService> logger) => _logger = logger;


    public async Task<DashboardDto> ProduceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Heavy processing started by {Thread}", Thread.CurrentThread.ManagedThreadId);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        var dto = new DashboardDto(
        Summary: "Key metrics snapshot",
        GeneratedAt: DateTime.UtcNow,
        Version: Guid.NewGuid());
        _logger.LogInformation("Heavy processing finished: {Version}", dto.Version);
        return dto;
    }
}

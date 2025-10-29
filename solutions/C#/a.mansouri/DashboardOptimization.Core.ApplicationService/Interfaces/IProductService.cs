namespace DashboardOptimization.Core.ApplicationService.Interfaces;
public interface IProductService
{
    Task<List<ProductModel>> GetProductsAsync();

    Task ClearCache();
}

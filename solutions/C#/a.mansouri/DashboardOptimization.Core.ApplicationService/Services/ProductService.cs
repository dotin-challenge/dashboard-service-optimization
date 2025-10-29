using DashboardOptimization.Core.ApplicationService.Extensions;
using DashboardOptimization.Core.ApplicationService.Interfaces;
using Microsoft.Extensions.Logging;

namespace DashboardOptimization.Core.ApplicationService.Services;
public class ProductService : IProductService
{
    private const string ProductCacheKey = "ProductCache";
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly ILogger<ProductService> _logger;
    private readonly ICacheStorageSpaceAdapter _cacheStorageSpaceAdapter;

    public ProductService(ILogger<ProductService> logger,
        ICacheStorageSpaceAdapter cacheStorageSpaceAdapter)
    {
        _logger = logger;
        _cacheStorageSpaceAdapter = cacheStorageSpaceAdapter;
    }

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        _logger.LogInformation("{@method} called.", MethodExtension.GetAsyncMethodName());
        (bool Result, List<ProductModel> Value) products = await _cacheStorageSpaceAdapter.TryGetValueAsync<List<ProductModel>>(ProductCacheKey);
        if (products.Result)
        {
            return products.Value;
        }

        await _semaphoreSlim.WaitAsync();
        try
        {
            products = await _cacheStorageSpaceAdapter.TryGetValueAsync<List<ProductModel>>(ProductCacheKey);
            if (products.Result)
            {
                return products.Value;
            }

            _logger.LogInformation("product not found in cache, get from DB, Then cache it.");
            var productsFromDb = await ProductRepository.GetProductModelsWithDelayAsync(TimeSpan.FromSeconds(5));
            await _cacheStorageSpaceAdapter.CreateEntryAsync(ProductCacheKey,
                productsFromDb,
                TimeSpan.FromHours(1),
                TimeSpan.FromSeconds(30));

            return productsFromDb;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "get product from cache has error");
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }   
    }

    public async Task ClearCache()
    {
        await _cacheStorageSpaceAdapter.RemoveAsync(ProductCacheKey);
    }
}

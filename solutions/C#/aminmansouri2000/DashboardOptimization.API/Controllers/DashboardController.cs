using DashboardOptimization.Core.ApplicationService;
using DashboardOptimization.Core.ApplicationService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DashboardOptimization.API.Controllers;
[ApiController]
[Route("[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IProductService _productService;

    public DashboardController(IProductService productService)
    {
        _productService = productService;
    }


    [HttpGet()]
    public async Task<List<ProductModel>> GetAsync()
    {
        var response = await _productService.GetProductsAsync();
        return response.ToList();
    }

    [HttpDelete("clear-cache")]
    public async Task ClearCacheAsync()
    {
        await _productService.ClearCache();
    }
}

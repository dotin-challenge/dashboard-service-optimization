namespace DashboardOptimization.Core.ApplicationService.Services;
internal static class ProductRepository
{
    public static async Task<List<ProductModel>> GetProductModelsWithDelayAsync(TimeSpan delay)
    {
        await Task.Delay(delay);
        return GetProductModels().ToList();
    }

    private static IEnumerable<ProductModel> GetProductModels()
    {
        for (int i = 1; i <= 10; i++)
        {
            yield return new ProductModel
            {
                Id = i,
                Name = Path.GetRandomFileName(),
                Count = Random.Shared.Next(100),
                CreateTime = DateTime.Now.AddDays(-i)
            };
        }
    }

}

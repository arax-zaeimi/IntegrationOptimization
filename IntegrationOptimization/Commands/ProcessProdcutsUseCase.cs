using IntegrationOptimization.ApiClients;
using IntegrationOptimization.Models;

namespace IntegrationOptimization.Commands;

public class ProcessProdcutsUseCase(ILogger<ProcessProdcutsUseCase> logger, IProductsApiClient apiClient) : IProcessProdcutsUseCase
{
    private const int TotalIterations = 200;
    public async Task ProcessNonOptimizedAsync()
    {
        List<Product> allProducucts = [];
        for (int i = 0; i < TotalIterations; i++)
        {
            var products = await apiClient.GetProductsNonOptimized();
            allProducucts.AddRange(products.Products);
        }

        var index = 0;
        foreach (var product in allProducucts)
        {
            logger.LogInformation("Processed product: {Index}-{Title}", index++, product.Title);
        }
    }

    public async Task ProcessOptimizedAsync()
    {
        var index = 0;
        await foreach (var product in FetchProducts())
        {
            logger.LogInformation("Processed product: {Index}-{Title}", index++, product.Title);
        }
    }

    private async IAsyncEnumerable<Product> FetchProducts()
    {
        for (int i = 0; i < TotalIterations; i++)
        {
            var productsResponse = await apiClient.GetProductsOptimized();
            foreach (var product in productsResponse.Products)
            {
                yield return product;
            }
        }
    }
}

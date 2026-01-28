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
            var apiResponse = await apiClient.GetProductsNonOptimized();
            if (apiResponse.IsFailure)
            {
                logger.LogError("Failed to fetch products on iteration {Iteration}: {Error}", i, apiResponse.Error);
                continue; // Skip this iteration but continue processing
            }
            allProducucts.AddRange(apiResponse.Value!.Products);
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
        // Use IAsyncEnumerable to process products as they are fetched
        // Don't accumulate all products in memory
        // Process each product as it is yielded
        await foreach (var product in FetchProductsOptimized())
        {
            logger.LogInformation("Processed product: {Index}-{Title}", index++, product.Title);
        }
    }

    public async Task ProcessSuperOptimizedAsync()
    {
        var index = 0;
        // Use IAsyncEnumerable to process products as they are fetched
        // Don't accumulate all products in memory
        // Process each product as it is yielded
        await foreach (var product in FetchProductsSuperOptimized())
        {
            logger.LogInformation("Processed product: {Index}-{Title}", index++, product.Title);
        }
    }

    public async Task ProcessInvoice()
    {
        await foreach (var item in apiClient.ReadInvoiceItemsFromCompressedFileAsync("Assets\\invoice_reconciliation_20251216_032532.jsonl.gz"))
        {
            logger.LogInformation("Processed invoice item for customer: {CustomerId} - {CustomerName}", item.CustomerId, item.CustomerName);
        }
    }

    private async IAsyncEnumerable<Product> FetchProductsOptimized()
    {
        // Fetch and process products in a streaming manner
        for (int i = 0; i < TotalIterations; i++)
        {
            var apiResponse = await apiClient.GetProductsOptimized();
            if (apiResponse.IsFailure)
            {
                logger.LogError("Failed to fetch products on iteration {Iteration}: {Error}", i, apiResponse.Error);
                continue;
            }
            foreach (var product in apiResponse.Value!.Products)
            {
                // Yield each product as it is processed
                yield return product;
            }
        }
    }

    private async IAsyncEnumerable<Product> FetchProductsSuperOptimized()
    {
        // Fetch and process products in a streaming manner
        for (int i = 0; i < TotalIterations; i++)
        {
            var apiResponse = await apiClient.GetProductsSuperOptimized();
            if (apiResponse.IsFailure)
            {
                logger.LogError("Failed to fetch products on iteration {Iteration}: {Error}", i, apiResponse.Error);
                continue;
            }
            foreach (var product in apiResponse.Value!.Products)
            {
                // Yield each product as it is processed
                yield return product;
            }
        }
    }
}

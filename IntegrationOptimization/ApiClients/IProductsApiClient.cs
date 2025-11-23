using IntegrationOptimization.Models;

namespace IntegrationOptimization.ApiClients
{
    public interface IProductsApiClient
    {
        Task<ProductResponse> GetProductsNonOptimized();
        Task<ProductResponse> GetProductsOptimized();
    }
}
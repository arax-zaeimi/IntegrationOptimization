using IntegrationOptimization.Models;

namespace IntegrationOptimization.ApiClients
{
    public interface IProductsApiClient
    {
        public Task<ApiResponse<ProductResponse>> GetProductsNonOptimized();
        public Task<ApiResponse<ProductResponse>> GetProductsOptimized();
    }
}
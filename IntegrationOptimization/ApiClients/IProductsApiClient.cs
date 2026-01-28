using IntegrationOptimization.Models;

namespace IntegrationOptimization.ApiClients
{
    public interface IProductsApiClient
    {
        public Task<ApiResponse<ProductResponse>> GetProductsNonOptimized();
        public Task<ApiResponse<ProductResponse>> GetProductsOptimized();
        public Task<ApiResponse<ProductResponse>> GetProductsSuperOptimized();
        public IAsyncEnumerable<InvoiceItemLine> ReadInvoiceItemsFromCompressedFileAsync(string filePath, CancellationToken cancellationToken = default);
        public IAsyncEnumerable<InvoiceItemLine> ReadInvoiceItemsFromCloudStorageAsync(string fileUrl, ProcessingOptions? options = null, CancellationToken cancellationToken = default);
        public Task<CloudFileMetadata> GetFileMetadataAsync(string fileUrl, CancellationToken cancellationToken = default);
    }
}
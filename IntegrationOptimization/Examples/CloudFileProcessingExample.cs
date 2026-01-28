using IntegrationOptimization.ApiClients;
using IntegrationOptimization.Models;

namespace IntegrationOptimization.Examples;

public class CloudFileProcessingExample
{
    private readonly IProductsApiClient _productsApiClient;
    private readonly ILogger<CloudFileProcessingExample> _logger;

    public CloudFileProcessingExample(IProductsApiClient productsApiClient, ILogger<CloudFileProcessingExample> logger)
    {
        _productsApiClient = productsApiClient;
        _logger = logger;
    }

    public async Task ProcessCloudInvoiceFileExample(string fileUrl)
    {
        try
        {
            // Get file metadata first
            var metadata = await _productsApiClient.GetFileMetadataAsync(fileUrl);
            
            _logger.LogInformation("File metadata: Size={Size}bytes, SupportsRanges={SupportsRanges}, ContentType={ContentType}",
                metadata.ContentLength, metadata.SupportsRangeRequests, metadata.ContentType);

            // Configure processing options based on file size
            var options = new ProcessingOptions
            {
                MaxDegreeOfParallelism = metadata.ContentLength > 10_000_000 ? 8 : 4, // Use more parallelism for larger files
                ChunkSizeBytes = 2 * 1024 * 1024, // 2MB chunks
                MaxRetries = 3,
                RetryDelay = TimeSpan.FromSeconds(2),
                HttpTimeout = TimeSpan.FromMinutes(10)
            };

            var totalProcessed = 0;
            var totalAmount = 0m;

            // Process the file
            await foreach (var invoiceItem in _productsApiClient.ReadInvoiceItemsFromCloudStorageAsync(fileUrl, options))
            {
                // Process each invoice item
                _logger.LogDebug("Processing invoice: {InvoiceNumber} for {CustomerName} - {Total} {Currency}",
                    invoiceItem.InvoiceNumber, invoiceItem.CustomerName, invoiceItem.Total, invoiceItem.Currency);

                totalAmount += invoiceItem.Total;
                totalProcessed++;

                // Log progress every 1000 items
                if (totalProcessed % 1000 == 0)
                {
                    _logger.LogInformation("Processed {Count} items so far. Running total: {TotalAmount}",
                        totalProcessed, totalAmount);
                }

                // Example: You can process each item here - save to database, validate, etc.
                await ProcessInvoiceItem(invoiceItem);
            }

            _logger.LogInformation("Completed processing {TotalCount} invoice items. Total amount: {TotalAmount}",
                totalProcessed, totalAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing cloud invoice file: {FileUrl}", fileUrl);
            throw;
        }
    }

    private async Task ProcessInvoiceItem(InvoiceItemLine invoiceItem)
    {
        // Example processing logic
        // This could include:
        // - Validation
        // - Database storage
        // - Business rule application
        // - Integration with other systems
        
        await Task.Delay(1); // Simulate some processing time
        
        // Example validation
        if (invoiceItem.Total < 0)
        {
            _logger.LogWarning("Negative total found in invoice {InvoiceNumber}: {Total}",
                invoiceItem.InvoiceNumber, invoiceItem.Total);
        }
    }
}
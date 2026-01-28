using IntegrationOptimization.Commands;
using IntegrationOptimization.ApiClients;
using IntegrationOptimization.Models;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationOptimization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProcessProdcutsUseCase processProductsUseCase, 
    IProductsApiClient productsApiClient,
    ILogger<ProductsController> logger) : ControllerBase
{
    [HttpPost("nonoptimized-process")]
    public IActionResult ProcessProductsNonOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessNonOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during non-optimized processing");
                }
            });

            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }

    [HttpPost("optimized-process")]
    public IActionResult ProcessProductsOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during optimized processing");
                }
            });

            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }

    [HttpPost("superoptimized-process")]
    public IActionResult ProcessProductsSuperOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessSuperOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during super-optimized processing");
                }
            });
            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }

    [HttpPost("process-invoice")]
    public IActionResult ProcessInvoice()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessInvoice();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during invoice processing");
                }
            });
            return Ok(new { Message = "Invoice processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting invoice processing", Details = ex.Message });
        }
    }

    [HttpPost("process-cloud-invoice")]
    public async Task<IActionResult> ProcessCloudInvoiceAsync([FromBody] CloudInvoiceRequest request)
    {
        try
        {
            // Get file metadata first
            var metadata = await productsApiClient.GetFileMetadataAsync(request.FileUrl);
            
            logger.LogInformation("Processing file: {FileUrl}, Size: {Size} bytes, Supports Range: {SupportsRange}",
                request.FileUrl, metadata.ContentLength, metadata.SupportsRangeRequests);

            var options = new ProcessingOptions
            {
                MaxDegreeOfParallelism = request.MaxParallelism ?? Environment.ProcessorCount,
                ChunkSizeBytes = request.ChunkSizeBytes ?? 1024 * 1024, // 1MB default
                MaxRetries = request.MaxRetries ?? 3,
                RetryDelay = TimeSpan.FromSeconds(request.RetryDelaySeconds ?? 1),
                HttpTimeout = TimeSpan.FromMinutes(request.TimeoutMinutes ?? 5)
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    var itemCount = 0;
                    var processedCount = 0;
                    
                    await foreach (var invoiceItem in productsApiClient.ReadInvoiceItemsFromCloudStorageAsync(
                        request.FileUrl, options))
                    {
                        itemCount++;
                        
                        // Process the invoice item here
                        logger.LogDebug("Processing invoice item: {InvoiceNumber} - {Total} {Currency}",
                            invoiceItem.InvoiceNumber, invoiceItem.Total, invoiceItem.Currency);
                        
                        processedCount++;
                        
                        // Log progress periodically
                        if (processedCount % 1000 == 0)
                        {
                            logger.LogInformation("Processed {Count} invoice items so far", processedCount);
                        }
                    }
                    
                    logger.LogInformation("Successfully processed {TotalCount} invoice items from {FileUrl}",
                        processedCount, request.FileUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing cloud invoice file: {FileUrl}", request.FileUrl);
                }
            });

            return Ok(new
            {
                Message = "Cloud invoice processing started successfully",
                FileMetadata = new
                {
                    Size = metadata.ContentLength,
                    SupportsRangeRequests = metadata.SupportsRangeRequests,
                    ContentType = metadata.ContentType,
                    LastModified = metadata.LastModified
                },
                ProcessingOptions = options
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting cloud invoice processing for: {FileUrl}", request.FileUrl);
            return StatusCode(500, new { Error = "An error occurred while starting cloud invoice processing", Details = ex.Message });
        }
    }

    [HttpGet("file-metadata")]
    public async Task<IActionResult> GetFileMetadataAsync([FromQuery] string fileUrl)
    {
        try
        {
            var metadata = await productsApiClient.GetFileMetadataAsync(fileUrl);
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting file metadata for: {FileUrl}", fileUrl);
            return StatusCode(500, new { Error = "An error occurred while getting file metadata", Details = ex.Message });
        }
    }
}
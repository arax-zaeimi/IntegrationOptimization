using IntegrationOptimization.Models;
using IntegrationOptimization.Services;
using System.Buffers;
using System.Text.Json;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace IntegrationOptimization.ApiClients;

public class ProductsApiClient(IHttpClientFactory httpClientFactory) : IProductsApiClient
{
    private const string ApiClientName = "DummyJsonApi";
    private readonly CloudFileProcessor _cloudFileProcessor = new(httpClientFactory);

    public async Task<ApiResponse<ProductResponse>> GetProductsNonOptimized()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://dummyjson.com/products");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return ApiResponse<ProductResponse>.Failure($"Request failed with status code {response.StatusCode}");
        }

        var productResponse = JsonSerializer.Deserialize<ProductResponse>(responseContent);
        if (productResponse == null)
        {
            return ApiResponse<ProductResponse>.Failure("Failed to deserialize product response.");
        }

        return ApiResponse<ProductResponse>.Success(productResponse);
    }

    public async Task<ApiResponse<ProductResponse>> GetProductsOptimized()
    {
        // Use named HttpClient for better performance and connection pooling
        var client = httpClientFactory.CreateClient(ApiClientName);

        // Properly dispose the response to free memory quickly
        using var response = await client.GetAsync("products");

        if (!response.IsSuccessStatusCode)
        {
            return ApiResponse<ProductResponse>.Failure($"Request failed with status code {response.StatusCode}");
        }

        // Use the content stream directly with optimized JsonSerializer options
        using var stream = await response.Content.ReadAsStreamAsync();

        var productResponse = await JsonSerializer.DeserializeAsync<ProductResponse>(
            stream,
            // Optimize buffer size for better performance
            new JsonSerializerOptions()
            {
                DefaultBufferSize = 4096
            });

        if (productResponse == null)
        {
            return ApiResponse<ProductResponse>.Failure("Failed to deserialize product response.");
        }

        return ApiResponse<ProductResponse>.Success(productResponse);
    }

    public async Task<ApiResponse<ProductResponse>> GetProductsSuperOptimized()
    {
        // Use named HttpClient for better performance and connection pooling
        var client = httpClientFactory.CreateClient(ApiClientName);
        
        // Use ArrayPool to rent a buffer - this avoids allocations
        byte[] buffer = ArrayPool<byte>.Shared.Rent(65536); // 64KB buffer
        
        try
        {
            using var response = await client.GetAsync("products");
            
            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<ProductResponse>.Failure($"Request failed with status code {response.StatusCode}");
            }
            
            using var stream = await response.Content.ReadAsStreamAsync();
            
            // Read the entire response into our rented buffer using Memory<T>
            int totalBytesRead = 0;
            int bufferOffset = 0;
            
            while (true)
            {
                // Create a Memory<byte> slice from our buffer starting at the current offset
                Memory<byte> bufferSlice = buffer.AsMemory(bufferOffset);
                
                // Read directly into the Memory<byte> - zero-copy operation
                int bytesRead = await stream.ReadAsync(bufferSlice);
                
                if (bytesRead == 0)
                    break; // End of stream
                
                bufferOffset += bytesRead;
                totalBytesRead += bytesRead;
                
                // If we're running out of buffer space, we need to resize
                if (bufferOffset >= buffer.Length - 1024) // Leave some margin
                {
                    // Return current buffer and rent a larger one
                    byte[] oldBuffer = buffer;
                    buffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                    
                    // Copy existing data to new buffer using Memory<T> spans
                    oldBuffer.AsSpan(0, totalBytesRead).CopyTo(buffer.AsSpan());
                    
                    ArrayPool<byte>.Shared.Return(oldBuffer);
                    bufferOffset = totalBytesRead;
                }
            }
            
            // Create a ReadOnlyMemory<byte> that represents only the actual data we read
            ReadOnlyMemory<byte> jsonData = buffer.AsMemory(0, totalBytesRead);
            
            // Use the new JsonSerializer.Deserialize overload that accepts ReadOnlySpan<byte>
            var productResponse = JsonSerializer.Deserialize<ProductResponse>(
                jsonData.Span, // Convert Memory to Span for JSON deserialization
                new JsonSerializerOptions
                {
                    DefaultBufferSize = 4096,
                    PropertyNameCaseInsensitive = true
                });
            
            if (productResponse == null)
            {
                return ApiResponse<ProductResponse>.Failure("Failed to deserialize product response.");
            }
            
            return ApiResponse<ProductResponse>.Success(productResponse);
        }
        finally
        {
            // Always return the rented buffer to the pool
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
    
    public async IAsyncEnumerable<InvoiceItemLine> ReadInvoiceItemsFromCompressedFileAsync(
        string filePath, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        using var streamReader = new StreamReader(gzipStream);
        
        string? line;
        while ((line = await streamReader.ReadLineAsync(cancellationToken)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            InvoiceItemLine? invoiceItem = null;
            try
            {
                invoiceItem = JsonSerializer.Deserialize<InvoiceItemLine>(line, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                // Skip malformed JSON lines - you might want to log this in a real application
                continue;
            }
            
            if (invoiceItem != null)
            {
                yield return invoiceItem;
            }
        }
    }

    public async IAsyncEnumerable<InvoiceItemLine> ReadInvoiceItemsFromCloudStorageAsync(
        string fileUrl,
        ProcessingOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        options ??= new ProcessingOptions();
        
        await foreach (var item in _cloudFileProcessor.ProcessFileWithRetryAsync<InvoiceItemLine>(fileUrl, options, cancellationToken))
        {
            yield return item;
        }
    }

    public async Task<CloudFileMetadata> GetFileMetadataAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        return await _cloudFileProcessor.GetFileMetadataAsync(fileUrl, cancellationToken);
    }
}

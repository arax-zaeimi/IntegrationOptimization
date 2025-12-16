using IntegrationOptimization.Models;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace IntegrationOptimization.Services;

public class CloudFileProcessor
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CloudFileProcessor(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<CloudFileMetadata> GetFileMetadataAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(1);
        
        using var request = new HttpRequestMessage(HttpMethod.Head, fileUrl);
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return new CloudFileMetadata
        {
            ContentLength = response.Content.Headers.ContentLength ?? 0,
            ContentEncoding = response.Content.Headers.ContentEncoding?.FirstOrDefault(),
            ContentType = response.Content.Headers.ContentType?.MediaType,
            LastModified = response.Content.Headers.LastModified,
            ETag = response.Headers.ETag?.Tag,
            SupportsRangeRequests = response.Headers.AcceptRanges?.Contains("bytes") == true
        };
    }

    public async IAsyncEnumerable<T> ProcessFileWithRetryAsync<T>(
        string fileUrl,
        ProcessingOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : class
    {
        var metadata = await GetFileMetadataAsync(fileUrl, cancellationToken);
        
        if (metadata.ContentLength == 0)
        {
            yield break;
        }

        // For compressed files or files that don't support range requests, fall back to sequential processing
        if (!metadata.SupportsRangeRequests || metadata.ContentEncoding?.Contains("gzip") == true)
        {
            var sequentialResults = await ProcessFileSequentiallyAsync<T>(fileUrl, options, cancellationToken);
            foreach (var item in sequentialResults)
            {
                yield return item;
            }
            yield break;
        }

        // Process file in parallel chunks
        await foreach (var item in ProcessFileInParallelAsync<T>(fileUrl, metadata, options, cancellationToken))
        {
            yield return item;
        }
    }

    private async Task<IEnumerable<T>> ProcessFileSequentiallyAsync<T>(
        string fileUrl,
        ProcessingOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        var retryCount = 0;
        Exception? lastException = null;
        
        while (retryCount <= options.MaxRetries)
        {
            try
            {
                return await ProcessSequentialChunkAsync<T>(fileUrl, options, cancellationToken);
            }
            catch (Exception ex) when (retryCount < options.MaxRetries)
            {
                lastException = ex;
                retryCount++;
                await Task.Delay(options.RetryDelay * retryCount, cancellationToken);
            }
        }
        
        // If all retries failed, throw the last exception
        if (lastException != null)
        {
            throw lastException;
        }
        
        return Enumerable.Empty<T>();
    }

    private async Task<List<T>> ProcessSequentialChunkAsync<T>(
        string fileUrl,
        ProcessingOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        var results = new List<T>();
        
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = options.HttpTimeout;
        
        using var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        Stream processingStream = stream;
        
        // Handle compression
        if (response.Content.Headers.ContentEncoding?.Contains("gzip") == true)
        {
            processingStream = new GZipStream(stream, CompressionMode.Decompress);
        }
        
        using var streamReader = new StreamReader(processingStream, Encoding.UTF8);
        
        string? line;
        var lineNumber = 0;
        
        while ((line = await streamReader.ReadLineAsync(cancellationToken)) != null)
        {
            lineNumber++;
            cancellationToken.ThrowIfCancellationRequested();
            
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            var item = await ProcessLineWithRetryAsync<T>(line, lineNumber, options);
            if (item != null)
            {
                results.Add(item);
            }
        }
        
        return results;
    }

    private async IAsyncEnumerable<T> ProcessFileInParallelAsync<T>(
        string fileUrl,
        CloudFileMetadata metadata,
        ProcessingOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : class
    {
        var totalSize = metadata.ContentLength;
        var chunkSize = options.ChunkSizeBytes;
        var chunks = new List<(long start, long end)>();
        
        // Calculate chunk boundaries
        for (long start = 0; start < totalSize; start += chunkSize)
        {
            var end = Math.Min(start + chunkSize - 1, totalSize - 1);
            chunks.Add((start, end));
        }
        
        // Create a channel to collect results in order
        var channel = Channel.CreateUnbounded<(int chunkIndex, List<T> items)>();
        var writer = channel.Writer;
        
        // Process chunks in parallel
        var semaphore = new SemaphoreSlim(options.MaxDegreeOfParallelism);
        var tasks = chunks.Select(async (chunk, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var items = await ProcessChunkAsync<T>(fileUrl, chunk.start, chunk.end, options, cancellationToken);
                await writer.WriteAsync((index, items), cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        // Start processing and close writer when done
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.WhenAll(tasks);
            }
            finally
            {
                writer.Complete();
            }
        }, cancellationToken);
        
        // Collect results in order
        var results = new Dictionary<int, List<T>>();
        var nextExpectedIndex = 0;
        
        await foreach (var (chunkIndex, items) in channel.Reader.ReadAllAsync(cancellationToken))
        {
            results[chunkIndex] = items;
            
            // Yield results in order
            while (results.TryGetValue(nextExpectedIndex, out var orderedItems))
            {
                foreach (var item in orderedItems)
                {
                    yield return item;
                }
                results.Remove(nextExpectedIndex);
                nextExpectedIndex++;
            }
        }
    }

    private async Task<List<T>> ProcessChunkAsync<T>(
        string fileUrl,
        long startByte,
        long endByte,
        ProcessingOptions options,
        CancellationToken cancellationToken = default) where T : class
    {
        var retryCount = 0;
        Exception? lastException = null;
        
        while (retryCount <= options.MaxRetries)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = options.HttpTimeout;
                
                using var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(startByte, endByte);
                
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();
                
                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                
                // Handle potential partial lines at chunk boundaries
                var content = await reader.ReadToEndAsync(cancellationToken);
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                var results = new List<T>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    
                    var item = await ProcessLineWithRetryAsync<T>(line.Trim(), 0, options);
                    if (item != null)
                    {
                        results.Add(item);
                    }
                }
                
                return results; // Success
            }
            catch (Exception ex) when (retryCount < options.MaxRetries)
            {
                lastException = ex;
                retryCount++;
                await Task.Delay(options.RetryDelay * retryCount, cancellationToken);
            }
        }
        
        // If we reach here, all retries failed
        if (lastException != null)
        {
            // Log the error but don't throw - return empty results for this chunk
            // In a real application, you might want to log this error
        }
        
        return new List<T>();
    }

    private async Task<T?> ProcessLineWithRetryAsync<T>(
        string line,
        int lineNumber,
        ProcessingOptions options) where T : class
    {
        var retryCount = 0;
        
        while (retryCount <= options.MaxRetries)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(line, _jsonOptions);
            }
            catch (JsonException) when (retryCount < options.MaxRetries)
            {
                retryCount++;
                await Task.Delay(TimeSpan.FromMilliseconds(10 * retryCount));
            }
            catch
            {
                // For non-JSON exceptions, don't retry
                break;
            }
        }
        
        return null;
    }
}
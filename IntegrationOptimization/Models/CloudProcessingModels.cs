using System.Net.Http;

namespace IntegrationOptimization.Models;

public class CloudFileMetadata
{
    public long ContentLength { get; set; }
    public string? ContentEncoding { get; set; }
    public string? ContentType { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? ETag { get; set; }
    public bool SupportsRangeRequests { get; set; }
}

public class ProcessingOptions
{
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public int ChunkSizeBytes { get; set; } = 1024 * 1024; // 1MB default
    public int MaxRetries { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan HttpTimeout { get; set; } = TimeSpan.FromMinutes(5);
}

public class ProcessingResult<T>
{
    public IAsyncEnumerable<T> Items { get; }
    public IAsyncEnumerable<ProcessingError> Errors { get; }
    
    public ProcessingResult(IAsyncEnumerable<T> items, IAsyncEnumerable<ProcessingError> errors)
    {
        Items = items;
        Errors = errors;
    }
}

public class ProcessingError
{
    public long StartByte { get; set; }
    public long EndByte { get; set; }
    public int LineNumber { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public int RetryCount { get; set; }
}
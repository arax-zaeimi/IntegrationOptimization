namespace IntegrationOptimization.Models;

public class CloudInvoiceRequest
{
    public required string FileUrl { get; set; }
    public int? MaxParallelism { get; set; }
    public int? ChunkSizeBytes { get; set; }
    public int? MaxRetries { get; set; }
    public int? RetryDelaySeconds { get; set; }
    public int? TimeoutMinutes { get; set; }
}
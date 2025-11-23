using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class Review
{
    [JsonPropertyName("rating")]
    public int Rating { get; set; }
    
    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("reviewerName")]
    public string ReviewerName { get; set; } = string.Empty;
    
    [JsonPropertyName("reviewerEmail")]
    public string ReviewerEmail { get; set; } = string.Empty;
}
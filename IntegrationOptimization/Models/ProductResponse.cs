using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class ProductResponse
{
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("skip")]
    public int Skip { get; set; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
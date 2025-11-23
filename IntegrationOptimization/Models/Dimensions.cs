using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class Dimensions
{
    [JsonPropertyName("width")]
    public decimal Width { get; set; }
    
    [JsonPropertyName("height")]
    public decimal Height { get; set; }
    
    [JsonPropertyName("depth")]
    public decimal Depth { get; set; }
}
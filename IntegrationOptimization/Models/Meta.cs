using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class Meta
{
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = string.Empty;
    
    [JsonPropertyName("qrCode")]
    public string QrCode { get; set; } = string.Empty;
}
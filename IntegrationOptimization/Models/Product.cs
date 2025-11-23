using System.Text.Json.Serialization;

namespace IntegrationOptimization.Models;

public class Product
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("discountPercentage")]
    public decimal DiscountPercentage { get; set; }
    
    [JsonPropertyName("rating")]
    public decimal Rating { get; set; }
    
    [JsonPropertyName("stock")]
    public int Stock { get; set; }
    
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
    
    [JsonPropertyName("brand")]
    public string Brand { get; set; } = string.Empty;
    
    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;
    
    [JsonPropertyName("weight")]
    public int Weight { get; set; }
    
    [JsonPropertyName("dimensions")]
    public Dimensions Dimensions { get; set; } = new();
    
    [JsonPropertyName("warrantyInformation")]
    public string WarrantyInformation { get; set; } = string.Empty;
    
    [JsonPropertyName("shippingInformation")]
    public string ShippingInformation { get; set; } = string.Empty;
    
    [JsonPropertyName("availabilityStatus")]
    public string AvailabilityStatus { get; set; } = string.Empty;
    
    [JsonPropertyName("reviews")]
    public List<Review> Reviews { get; set; } = new();
    
    [JsonPropertyName("returnPolicy")]
    public string ReturnPolicy { get; set; } = string.Empty;
    
    [JsonPropertyName("minimumOrderQuantity")]
    public int MinimumOrderQuantity { get; set; }
    
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = new();
    
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;
    
    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = new();
}
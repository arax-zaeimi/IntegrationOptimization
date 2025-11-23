using IntegrationOptimization.Models;
using System.Text.Json;

namespace IntegrationOptimization.ApiClients;

public class ProductsApiClient(IHttpClientFactory httpClientFactory) : IProductsApiClient
{
    private const string ApiClientName = "DummyJsonApi";
    public async Task<ProductResponse> GetProductsNonOptimized()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://dummyjson.com/products");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        return JsonSerializer.Deserialize<ProductResponse>(responseContent)
            ?? throw new InvalidOperationException("Failed to deserialize product response.");
    }

    public async Task<ProductResponse> GetProductsOptimized()
    {
        // Use named HttpClient for better performance and connection pooling
        var client = httpClientFactory.CreateClient(ApiClientName);
        
        // Properly dispose the response to free memory quickly
        using var response = await client.GetAsync("products");
        
        // Check status before processing content
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        // Use the content stream directly with optimized JsonSerializer options
        using var stream = await response.Content.ReadAsStreamAsync();
        
        return await JsonSerializer.DeserializeAsync<ProductResponse>(stream, new JsonSerializerOptions() { DefaultBufferSize = 4096 }) 
            ?? throw new InvalidOperationException("Failed to deserialize product response.");
    }
}

using IntegrationOptimization.Models;
using System.Text.Json;

namespace IntegrationOptimization.ApiClients;

public class ProductsApiClient(IHttpClientFactory httpClientFactory) : IProductsApiClient
{
    private const string ApiClientName = "DummyJsonApi";
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
}

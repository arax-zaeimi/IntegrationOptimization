using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Engines;
using IntegrationOptimization.ApiClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IntegrationOptimization.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[SimpleJob(RunStrategy.ColdStart, iterationCount: 5)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MarkdownExporter, HtmlExporter]
public class ApiClientBenchmark
{
    private IProductsApiClient _apiClient = null!;
    private ServiceProvider _serviceProvider = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        // Configure logging with minimal output for benchmarks
        services.AddLogging(builder => 
            builder.AddConsole()
                   .SetMinimumLevel(LogLevel.Error)); // Minimal logging for cleaner benchmark results
        
        // Configure HttpClient with the same settings as the main application
        services.AddHttpClient("DummyJsonApi", client =>
        {
            client.BaseAddress = new Uri("https://dummyjson.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // Register API client
        services.AddScoped<IProductsApiClient, ProductsApiClient>();
        
        _serviceProvider = services.BuildServiceProvider();
        _apiClient = _serviceProvider.GetRequiredService<IProductsApiClient>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("ApiClient")]
    public async Task GetProductsNonOptimized()
    {
        var response = await _apiClient.GetProductsNonOptimized();
        // Ensure the response is consumed to avoid any lazy evaluation effects
        _ = response.IsSuccess;
    }

    [Benchmark]
    [BenchmarkCategory("ApiClient")]
    public async Task GetProductsOptimized()
    {
        var response = await _apiClient.GetProductsOptimized();
        // Ensure the response is consumed to avoid any lazy evaluation effects
        _ = response.IsSuccess;
    }

    [Benchmark]
    [BenchmarkCategory("ApiClient")]
    public async Task GetProductsSuperOptimized()
    {
        var response = await _apiClient.GetProductsSuperOptimized();
        // Ensure the response is consumed to avoid any lazy evaluation effects
        _ = response.IsSuccess;
    }
}
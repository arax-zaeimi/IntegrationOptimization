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

/// <summary>
/// Lightweight benchmark for quick performance comparison of the three API client methods.
/// Uses fewer iterations to reduce API load while still providing meaningful performance metrics.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, iterationCount: 3, warmupCount: 1)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[RankColumn]
public class QuickApiClientBenchmark
{
    private IProductsApiClient _apiClient = null!;
    private ServiceProvider _serviceProvider = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        // Minimal logging for clean benchmark output
        services.AddLogging(builder => 
            builder.SetMinimumLevel(LogLevel.Critical));
        
        // Configure HttpClient
        services.AddHttpClient("DummyJsonApi", client =>
        {
            client.BaseAddress = new Uri("https://dummyjson.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        services.AddScoped<IProductsApiClient, ProductsApiClient>();
        
        _serviceProvider = services.BuildServiceProvider();
        _apiClient = _serviceProvider.GetRequiredService<IProductsApiClient>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }

    [Benchmark(Baseline = true, Description = "Non-Optimized (new HttpClient)")]
    public async Task<bool> NonOptimized()
    {
        var response = await _apiClient.GetProductsNonOptimized();
        return response.IsSuccess;
    }

    [Benchmark(Description = "Optimized (HttpClientFactory + Stream)")]
    public async Task<bool> Optimized()
    {
        var response = await _apiClient.GetProductsOptimized();
        return response.IsSuccess;
    }

    [Benchmark(Description = "Super Optimized (Memory<T> + ArrayPool)")]
    public async Task<bool> SuperOptimized()
    {
        var response = await _apiClient.GetProductsSuperOptimized();
        return response.IsSuccess;
    }
}
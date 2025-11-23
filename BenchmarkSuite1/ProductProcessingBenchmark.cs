using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using IntegrationOptimization.ApiClients;
using IntegrationOptimization.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IntegrationOptimization.Benchmarks;

[MemoryDiagnoser]
public class ProductProcessingBenchmark
{
    private IProcessProdcutsUseCase _processProductsUseCase = null !;
    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        // Configure logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        // Register services
        services.AddHttpClient();
        services.AddScoped<IProductsApiClient, ProductsApiClient>();
        services.AddScoped<IProcessProdcutsUseCase, ProcessProdcutsUseCase>();
        var serviceProvider = services.BuildServiceProvider();
        _processProductsUseCase = serviceProvider.GetRequiredService<IProcessProdcutsUseCase>();
    }

    [Benchmark]
    public async Task ProcessProductsNonOptimized()
    {
        await _processProductsUseCase.ProcessNonOptimizedAsync();
    }

    [Benchmark]
    public async Task ProcessProductsOptimized()
    {
        await _processProductsUseCase.ProcessOptimizedAsync();
    }
}
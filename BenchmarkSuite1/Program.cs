using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using IntegrationOptimization.Benchmarks;
using System;

namespace BenchmarkSuite1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create custom configuration for better reporting
            var config = ManualConfig.Create(DefaultConfig.Instance)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddDiagnoser(ThreadingDiagnoser.Default)
                .AddExporter(MarkdownExporter.GitHub)
                .AddExporter(HtmlExporter.Default);

            Console.WriteLine("=== Integration Optimization Benchmarks ===");
            Console.WriteLine();
            Console.WriteLine("Choose benchmark to run:");
            Console.WriteLine("1. Quick API Client Comparison (Lightweight - 3 iterations)");
            Console.WriteLine("2. Detailed API Client Methods Comparison");
            Console.WriteLine("3. Full Processing Pipeline Comparison (200 iterations each)");
            Console.WriteLine("4. Run All Benchmarks");
            Console.WriteLine("5. Exit");
            Console.WriteLine();
            Console.Write("Enter your choice (1-5): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n=== Running Quick API Client Benchmark ===");
                    Console.WriteLine("This benchmark uses minimal iterations for fast comparison.");
                    BenchmarkRunner.Run<QuickApiClientBenchmark>(config);
                    break;
                    
                case "2":
                    Console.WriteLine("\n=== Running Detailed API Client Benchmarks ===");
                    BenchmarkRunner.Run<ApiClientBenchmark>(config);
                    break;
                    
                case "3":
                    Console.WriteLine("\n=== Running Full Processing Pipeline Benchmarks ===");
                    Console.WriteLine("Warning: This will make 600 API calls total (3 methods × 200 iterations). This may take several minutes.");
                    Console.Write("Continue? (y/N): ");
                    var confirm = Console.ReadLine();
                    if (confirm?.ToLower() == "y")
                    {
                        BenchmarkRunner.Run<ProductProcessingBenchmark>(config);
                    }
                    else
                    {
                        Console.WriteLine("Benchmark cancelled.");
                    }
                    break;
                    
                case "4":
                    Console.WriteLine("\n=== Running All Benchmarks ===");
                    Console.WriteLine("Warning: This will make many API calls and may take significant time.");
                    Console.Write("Continue? (y/N): ");
                    var confirmAll = Console.ReadLine();
                    if (confirmAll?.ToLower() == "y")
                    {
                        BenchmarkRunner.Run<QuickApiClientBenchmark>(config);
                        BenchmarkRunner.Run<ApiClientBenchmark>(config);
                        BenchmarkRunner.Run<ProductProcessingBenchmark>(config);
                    }
                    else
                    {
                        Console.WriteLine("Benchmarks cancelled.");
                    }
                    break;
                    
                case "5":
                    Console.WriteLine("Exiting...");
                    return;
                    
                default:
                    Console.WriteLine("Invalid choice. Running quick benchmark...");
                    BenchmarkRunner.Run<QuickApiClientBenchmark>(config);
                    break;
            }

            Console.WriteLine("\nBenchmarks completed! Check the BenchmarkDotNet.Artifacts folder for detailed results.");
            Console.WriteLine("Results include:");
            Console.WriteLine("- Markdown files for GitHub-ready reports");
            Console.WriteLine("- HTML files for detailed browser viewing");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

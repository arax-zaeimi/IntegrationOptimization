# IntegrationOptimization

## Overview

This repository demonstrates the performance difference between **optimized** and **non-optimized** approaches in a .NET 10 Web API application. The project serves as a practical comparison tool to evaluate the impact of various optimization techniques on memory usage, execution time, and resource efficiency.

## API Endpoints

The Web API exposes two endpoints that perform the same task using different approaches:

### 1. `/api/products/nonoptimized-process` (POST)

Triggers the **non-optimized** approach that:

- Creates new HttpClient instances for each request
- Loads all API responses into memory at once
- Uses `ReadAsStringAsync()` for content processing
- Stores all products in a single large collection before processing

### 2. `/api/products/optimized-process` (POST)

Triggers the **optimized** approach that:

- Uses named HttpClient with connection pooling
- Implements streaming with `IAsyncEnumerable<T>`
- Uses `ReadAsStreamAsync()` with buffered JSON deserialization
- Processes products one-by-one using `yield return` pattern
- Properly disposes of HTTP responses

## Key Optimization Techniques Demonstrated

### HTTP Client Optimization

- **Non-optimized**: Creates new HttpClient instances
- **Optimized**: Uses `IHttpClientFactory` with named clients and connection pooling

### Memory Management

- **Non-optimized**: Loads entire dataset into memory (`List<Product>`)
- **Optimized**: Streams data using `IAsyncEnumerable<T>` and `yield return`

### JSON Processing

- **Non-optimized**: Reads response as string then deserializes
- **Optimized**: Direct stream deserialization with custom buffer size

### Resource Disposal

- **Non-optimized**: Relies on garbage collector
- **Optimized**: Explicit `using` statements for proper disposal

## Performance Testing

The project includes a comprehensive benchmark suite using BenchmarkDotNet that measures:

- Memory allocation and usage
- Execution time
- Garbage collection impact
- Resource utilization

## Intention

This repository serves to:

1. **Demonstrate** the real-world impact of optimization techniques in .NET applications
2. **Compare** memory usage and performance between optimized and non-optimized code
3. **Provide** a practical reference for .NET performance best practices
4. **Educate** developers on the importance of proper resource management and streaming techniques

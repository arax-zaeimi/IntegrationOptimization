```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7171)
Unknown processor
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-KFGNHS : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=5  RunStrategy=ColdStart  

```
| Method                    | Mean     | Error     | StdDev   | Median   | Min      | Max      | Ratio | RatioSD | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|-------------------------- |---------:|----------:|---------:|---------:|---------:|---------:|------:|--------:|---------------------:|-----------------:|----------:|------------:|
| GetProductsNonOptimized   | 92.76 ms | 140.83 ms | 36.57 ms | 78.00 ms | 73.62 ms | 158.1 ms |  1.09 |    0.48 |              14.0000 |                - | 247.06 KB |        1.00 |
| GetProductsOptimized      | 57.39 ms | 286.76 ms | 74.47 ms | 23.93 ms | 23.17 ms | 190.6 ms |  0.67 |    0.83 |               1.0000 |                - | 348.45 KB |        1.41 |
| GetProductsSuperOptimized | 54.05 ms | 246.00 ms | 63.88 ms | 26.46 ms | 24.46 ms | 168.3 ms |  0.63 |    0.72 |               1.0000 |                - | 344.79 KB |        1.40 |

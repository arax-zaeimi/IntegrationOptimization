```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7171)
Unknown processor
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-AYUXIY : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=3  RunStrategy=ColdStart  

```
| Method                        | Mean     | Error   | StdDev   | Min      | Max      | Median   | Ratio | Gen0      | Completed Work Items | Lock Contentions | Gen1      | Gen2      | Allocated | Alloc Ratio |
|------------------------------ |---------:|--------:|---------:|---------:|---------:|---------:|------:|----------:|---------------------:|-----------------:|----------:|----------:|----------:|------------:|
| ProcessProductsNonOptimized   | 15.725 s | 2.528 s | 0.1386 s | 15.605 s | 15.877 s | 15.694 s |  1.00 | 4000.0000 |            2718.0000 |                - | 3000.0000 | 3000.0000 |  48.37 MB |        1.00 |
| ProcessProductsOptimized      |  4.424 s | 2.078 s | 0.1139 s |  4.331 s |  4.551 s |  4.390 s |  0.28 | 3000.0000 |             224.0000 |                - | 1000.0000 |         - |  27.87 MB |        0.58 |
| ProcessProductsSuperOptimized |  4.271 s | 1.860 s | 0.1019 s |  4.201 s |  4.388 s |  4.224 s |  0.27 | 3000.0000 |             214.0000 |                - |         - |         - |  27.39 MB |        0.57 |

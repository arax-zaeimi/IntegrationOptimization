```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.7171)
Unknown processor
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-OIOVDM : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

IterationCount=3  RunStrategy=ColdStart  WarmupCount=1  

```
| Method                                    | Mean      | Error       | StdDev   | Median   | Min      | Max      | Ratio | RatioSD | Rank | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|------------------------------------------ |----------:|------------:|---------:|---------:|---------:|---------:|------:|--------:|-----:|---------------------:|-----------------:|----------:|------------:|
| &#39;Non-Optimized (new HttpClient)&#39;          | 125.05 ms | 1,495.54 ms | 81.98 ms | 84.17 ms | 71.55 ms | 219.4 ms |  1.27 |    0.94 |    2 |              10.0000 |                - | 549.88 KB |        1.00 |
| &#39;Optimized (HttpClientFactory + Stream)&#39;  |  75.57 ms | 1,588.24 ms | 87.06 ms | 25.68 ms | 24.92 ms | 176.1 ms |  0.77 |    0.88 |    1 |               1.0000 |                - |  348.2 KB |        0.63 |
| &#39;Super Optimized (Memory&lt;T&gt; + ArrayPool)&#39; |  70.16 ms | 1,462.14 ms | 80.14 ms | 24.53 ms | 23.24 ms | 162.7 ms |  0.71 |    0.81 |    1 |               1.0000 |                - | 347.45 KB |        0.63 |

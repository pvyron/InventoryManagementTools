using BenchmarkDotNet.Running;
using InMa.Benchmarks;


BenchmarkRunner.Run<Benchmark>();

var bm = new Benchmark();
bm.number = Benchmark.value1;

Console.WriteLine(bm.NewMine());
Console.WriteLine(bm.NewMine_Optimized());
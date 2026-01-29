using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance.AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()));

var summaries = BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args, config);
namespace Rufus.Benchmarks.Benchmarks;

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class Polymorphism
{
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("IsOk")]
    public bool PatternMatchingIsOk()
    {
        var result = new Result<int, string>.Ok(42);

        return result.IsOk;
    }

    [Benchmark]
    [BenchmarkCategory("IsOk")]
    public bool PolymorphicIsOk()
    {
        var result = new PolymorphicResult<int, string>.Ok(42);

        return result.IsOk;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Map")]
    public int PatternMatchingMap()
    {
        var result = new Result<string, string>.Ok("42");

        return result.Map(int.Parse) switch
        {
            Result.Ok<int>(var value) => value,
            _ => default,
        };
    }

    [Benchmark]
    [BenchmarkCategory("Map")]
    public int PolymorphicMap()
    {
        var result = new PolymorphicResult<string, string>.Ok("42");

        return result.Map(int.Parse) switch
        {
            Result.Ok<int>(var value) => value,
            _ => default,
        };
    }
}
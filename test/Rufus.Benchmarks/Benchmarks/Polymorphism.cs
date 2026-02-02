namespace Rufus.Benchmarks.Benchmarks;

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using FnResult = Result<int, string>;
using PolymorphicResult = Rufus.Benchmarks.Alt.PolymorphicResult<int, string>;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class Polymorphism
{
    [ParamsSource(nameof(Results))]
    public FnResult FnResult { get; set; } = null!;

    [ParamsSource(nameof(FnResults))]
    public PolymorphicResult PolymorphicResult { get; set; } = null!;

    public static IEnumerable<FnResult> Results
        => [new FnResult.Ok(42), new FnResult.Error("Error")];

    public static IEnumerable<PolymorphicResult> FnResults
        => [new PolymorphicResult.Ok(42), new PolymorphicResult.Error("Error")];

    [Benchmark]
    [BenchmarkCategory("IsError")]
    public bool FnIsError() => FnResult.IsError;

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("IsError")]
    public bool PolymorphicIsError() => PolymorphicResult.IsError;

    [Benchmark]
    [BenchmarkCategory("IsOk")]
    public bool FnIsOk() => FnResult.IsOk;

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("IsOk")]
    public bool PolymorphicIsOk() => PolymorphicResult.IsOk;

    [Benchmark]
    [BenchmarkCategory("IsErrorAnd")]
    public bool FnIsErrorAnd() => FnResult.IsErrorAnd(static _ => true);

    [Benchmark(Baseline =  true)]
    [BenchmarkCategory("IsErrorAnd")]
    public bool PolymorphicIsErrorAnd() => PolymorphicResult.IsErrorAnd(static _ => true);

    [Benchmark]
    [BenchmarkCategory("IsOkAnd")]
    public bool FnIsOkAnd() => FnResult.IsOkAnd(static x => x > 0);

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("IsOkAnd")]
    public bool PolymorphicIsOkAnd() => PolymorphicResult.IsOkAnd(static x => x > 0);

    [Benchmark]
    [BenchmarkCategory("Inspect")]
    public void FnInspect() => FnResult.Inspect(static _ => { });

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Inspect")]
    public void PolymorphicInspect() => PolymorphicResult.Inspect(static _ => { });

    [Benchmark]
    [BenchmarkCategory("Map")]
    public void FnMap() => FnResult.Map(static _ => "Mapped: {x}");

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Map")]
    public void PolymorphicMap() => PolymorphicResult.Map(static _ => "Mapped: {x}");

    [Benchmark]
    [BenchmarkCategory("MapOr")]
    public string FnMapOr() => FnResult.MapOr(static _ => "Mapped: {x}", string.Empty);

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MapOr")]
    public string PolymorphicMapOr() => PolymorphicResult.MapOr(static _ => "Mapped: {x}", string.Empty);

    [Benchmark]
    [BenchmarkCategory("MapOrElse")]
    public string FnMapOrElse() => FnResult.MapOrElse(static _ => "Mapped: {x}", static _ => string.Empty);

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MapOrElse")]
    public string PolymorphicMapOrElse()
        => PolymorphicResult.MapOrElse(static _ => "Mapped: {x}", static _ => string.Empty);

    [Benchmark]
    [BenchmarkCategory("MapError")]
    public void FnMapError() => FnResult.MapError(static _ => -1);

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("MapError")]
    public void PolymorphicMapError() => PolymorphicResult.MapError(static _ => -1);
}
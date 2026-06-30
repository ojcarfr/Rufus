namespace Rufus.Benchmarks.Benchmarks;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class SwitchCase
{
    [Benchmark(Baseline = true, Description = "Switch by generic Result type")]
    public int RecordCase()
    {
        Result<int, string> result = Do();

        return result switch
        {
            Ok<int>(var value) => value,
            Error<string> => int.MaxValue,
            _ => throw new SwitchExpressionException(),
        };
    }

    [Benchmark(Description = "Switch by interface case types")]
    public int InterfaceCase()
    {
        Result<int, string> result = Do();

        return result switch
        {
            Result<int, string>.Ok(var value) => value,
            Result<int, string>.Error => int.MaxValue,
            _ => throw new SwitchExpressionException(),
        };
    }

    private static Result<int, string> Do() => Result.Ok(5);
}
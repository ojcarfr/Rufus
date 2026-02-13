namespace Rufus.Tests;

public partial class AsyncResultTests
{
    private const string EXPECTED_ERROR = "Expected error";
    private const int OK_VALUE = 2;
    private static readonly ApplicationException ExpectedException = new(EXPECTED_ERROR);

    public static TheoryData<Promise<int, string>> ErrorPromises
        =>
        [
            new Promise<int, string>.Completed(Result.Error(EXPECTED_ERROR)),
            new Promise<int, string>.Pending(Result.Error(EXPECTED_ERROR)),
        ];

    public static TheoryData<Promise<int, string>> FaultedPromises =>
    [
        new Promise<int, string>.Completed(ExpectedException), new Promise<int, string>.Pending(ExpectedException),
    ];

    public static TheoryData<Promise<int, string>> SucceedPromises
        =>
        [
            new Promise<int, string>.Completed(Result.Ok(OK_VALUE)),
            new Promise<int, string>.Pending(Result.Ok(OK_VALUE)),
        ];

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnError()
    {
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnError()
    {
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Result.Ok(2);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Result.Ok("OK"));

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Ok("OK"), result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Result.Ok(2);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Result.Ok("OK"));

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Ok("OK"), result);
    }

    public abstract class Promise<T, TError>
        where T : notnull
        where TError : notnull
    {
        public abstract Task<Result<T, TError>> AsTask();

        public abstract ValueTask<Result<T, TError>> AsValueTask();

        public abstract void Ready();

        public sealed class Completed : Promise<T, TError>
        {
            private readonly Func<Result<T, TError>> _result;

            public Completed(Result<T, TError> result) => _result = () => result;

            public Completed(Exception exception) => _result = () => throw exception;

            public override Task<Result<T, TError>> AsTask()
            {
                try
                {
                    return Task.FromResult(_result());
                }
                catch (Exception exception)
                {
                    return Task.FromException<Result<T, TError>>(exception);
                }
            }

            public override ValueTask<Result<T, TError>> AsValueTask()
            {
                try
                {
                    return ValueTask.FromResult(_result());
                }
                catch (Exception exception)
                {
                    return ValueTask.FromException<Result<T, TError>>(exception);
                }
            }

            public override void Ready() { }
        }

        public sealed class Pending : Promise<T, TError>
        {
            private readonly Func<Result<T, TError>> _result  ;
            private readonly TaskCompletionSource<Result<T, TError>> _tcs = new();

            public Pending(Result<T, TError> result) => _result = () => result;

            public Pending(Exception exception) => _result = () => throw exception;

            public override Task<Result<T, TError>> AsTask() => _tcs.Task;

            public override ValueTask<Result<T, TError>> AsValueTask() => new(_tcs.Task);

            public override void Ready()
            {
                try
                {
                    _tcs.TrySetResult(_result());
                }
                catch (Exception exception)
                {
                    _tcs.TrySetException(exception);
                }
            }
        }
    }
}
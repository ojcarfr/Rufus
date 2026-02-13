namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;

public partial class AsyncResultTests
{
    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyErrorValueTaskAndAnyAsyncTaskFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        ValueTask<Result<string, string>> result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(Result.Error("Expected error"), await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyErrorValueTaskAndAnyAsyncValueTaskFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();

        ValueTask<Result<string, string>> result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(Result.Error("Expected error"), await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyErrorValueTaskAndAnyFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();

        ValueTask<Result<string, string>> result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(Result.Error("Expected error"), await result);
        fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyFaultedValueTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Task<Result<int, string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<int, string>> task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyFaultedValueTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<int, string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<int, string>> task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyFaultedValueTaskAndAnyFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Result<int, string>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<int, string>> task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueAndAnyFailingFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Throws(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<string, string>> t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Task.FromResult(expected));

        ValueTask<Result<string, string>> task = sut.AndThen(fn);
        promise.Ready();
        Result<string, string> result = await task;

        Assert.Equal(expected, result);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        #pragma warning disable CA2012
        fn.Invoke(OK_VALUE).Returns(ValueTask.FromResult(expected));
        #pragma warning restore CA2012

        ValueTask<Result<string, string>> task = sut.AndThen(fn);
        promise.Ready();
        Result<string, string> result = await task;

        Assert.Equal(expected, result);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueTaskAndAnyFailingAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<string, string>> t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueTaskAndAnyFailingAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            ValueTask<Result<string, string>> t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkValueTaskAndAnyFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        ValueTask<Result<int, string>> sut = promise.AsValueTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Returns(expected);

        ValueTask<Result<string, string>> task = sut.AndThen(fn);
        promise.Ready();
        Result<string, string> result = await task;

        Assert.Equal(expected, result);
    }
}
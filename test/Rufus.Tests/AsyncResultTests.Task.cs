namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;

public partial class AsyncResultTests
{
    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncTaskFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        Task<Result<string, string>> result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(Result.Error("Expected error"), await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncValueTaskFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
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
    public async Task GivenAnyErrorTaskAndAnyFunction_WhenAndThenOnTask_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();

        Task<Result<string, string>> result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(Result.Error("Expected error"), await result);
        fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<int, string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            Task<Result<int, string>> task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
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
    public async Task GivenAnyFaultedTaskAndAnyFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<int, string>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            Task<Result<int, string>> task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Task.FromResult(expected));

        Task<Result<string, string>> task = sut.AndThen(fn);
        promise.Ready();
        Result<string, string> result = await task;

        Assert.Equal(expected, result);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        Task<Result<int, string>> sut = promise.AsTask();
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
    public async Task GivenAnyOkTaskAndAnyFailingAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            Task<Result<string, string>> t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFailingAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
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
    public async Task GivenAnyOkTaskAndAnyFailingFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Throws(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            Task<Result<string, string>> t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(ExpectedException, thrown);
    }

    [Theory]
    #pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
    #pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Result.Ok("OK");
        Task<Result<int, string>> sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Returns(expected);

        Task<Result<string, string>> task = sut.AndThen(fn);
        promise.Ready();
        Result<string, string> result = await task;

        Assert.Equal(expected, result);
    }
}
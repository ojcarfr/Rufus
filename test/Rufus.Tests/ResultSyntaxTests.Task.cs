namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;
using Rufus.Tests.Utils;

public partial class ResultSyntaxTests
{
    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: Any.Error("Expected error"), actual: await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        var result = sut.OrElse(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnError(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: Any.Error("Expected error"), actual: await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        var result = sut.OrElse(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyFailingAsyncTaskFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.OrElse(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyFailingAsyncValueTaskFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.OrElse(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyFailingFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Result<int, int>>>();
        fn.Invoke(EXPECTED_ERROR).Throws(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.OrElse(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyFunction_WhenAndThen_ThenShouldReturnError(Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: Any.Error("Expected error"), actual: await result);
        fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(ErrorPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyErrorTaskAndAnyFunction_WhenOrElse_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Result<int, int>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        var result = sut.OrElse(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<int, string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.OrElse(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<int, string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.OrElse(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<int, string>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.AndThen(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTaskAndAnyFunction_WhenOrElse_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Result<int, int>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var task = sut.OrElse(fn);
            promise.Ready();
            await task;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Any.Ok("OK");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Task.FromResult(expected));

        var task = sut.AndThen(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnOkValue(
        Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();

        var task = sut.OrElse(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Any.Ok("OK");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
#pragma warning disable CA2012
        fn.Invoke(OK_VALUE).Returns(ValueTask.FromResult(expected));
#pragma warning restore CA2012

        var task = sut.AndThen(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnOkValue(
        Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();

        var task = sut.OrElse(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFailingAsyncTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFailingAsyncValueTaskFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFailingFunction_WhenAndThen_ThenShouldThrowException(
        Promise<int, string> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Throws(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFunction_WhenAndThen_ThenShouldReturnResultFromFunction(
        Promise<int, string> promise)
    {
        Result<string, string> expected = Any.Ok("OK");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(OK_VALUE).Returns(expected);

        var task = sut.AndThen(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SucceedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyOkTaskAndAnyFunction_WhenOrElse_ThenShouldReturnOkValue(Promise<int, string> promise)
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<string, Result<int, int>>>();

        var task = sut.OrElse(fn);
        promise.Ready();
        var result = await task;

        Assert.Equal(expected: expected, actual: result);
    }
}
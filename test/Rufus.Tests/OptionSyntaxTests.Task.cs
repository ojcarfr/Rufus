namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;
using Rufus.Tests.Utils;

public partial class OptionSyntaxTests
{
    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnNone(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.IsNone(await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<string> expected = Any.Some("Received");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();
        fn.Invoke(SOME_VALUE).Returns(expected);

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnNone(
        Promise<int> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Option<string>>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.IsNone(await result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<string> expected = Any.Some("Received");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, ValueTask<Option<string>>>>();
        fn.Invoke(SOME_VALUE).Returns(expected);

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnySyncFunction_WhenAndThen_ThenShouldReturnNone(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Option<string>>>();

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.IsNone(await result);
        fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnySyncFunction_WhenAndThen_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<string> expected = Any.Some("Received");
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Option<string>>>();
        fn.Invoke(SOME_VALUE).Returns(expected);

        var result = sut.AndThen(fn);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndFailingSyncFunction_WhenAndThen_ThenShouldThrowException(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Option<string>>>();
        fn.Invoke(SOME_VALUE).Throws(ExpectedException);

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
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTask_WhenAndThen_ThenShouldPropagateException(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.AndThen(fn);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnSome(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Task<Option<int>>>>();

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal<Option<int>>(expected: Any.Some(SOME_VALUE), actual: await result);
        await op.DidNotReceive().Invoke();
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<int> expected = Any.Some(42);
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Task<Option<int>>>>();
        op.Invoke().Returns(expected);

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnSome(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var op = Substitute.For<Func<ValueTask<Option<int>>>>();

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal<Option<int>>(expected: Any.Some(SOME_VALUE), actual: await result);
        await op.DidNotReceive().Invoke();
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<int> expected = Any.Some(42);
        var sut = promise.AsTask();
        var op = Substitute.For<Func<ValueTask<Option<int>>>>();
        op.Invoke().Returns(expected);

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(SomePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnySomeTaskAndAnySyncFunction_WhenOrElse_ThenShouldReturnSome(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Option<int>>>();

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal<Option<int>>(expected: Any.Some(SOME_VALUE), actual: await result);
        op.DidNotReceive().Invoke();
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndAnySyncFunction_WhenOrElse_ThenShouldReturnOptionFromFunction(
        Promise<int> promise)
    {
        Option<int> expected = Any.Some(42);
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Option<int>>>();
        op.Invoke().Returns(expected);

        var result = sut.OrElse(op);
        promise.Ready();

        Assert.Equal(expected: expected, actual: await result);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(NonePromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyNoneTaskAndFailingSyncFunction_WhenOrElse_ThenShouldThrowException(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Option<int>>>();
        op.Invoke().Throws(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.OrElse(op);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Theory]
#pragma warning disable xUnit1045
    [MemberData(nameof(FaultedPromises))]
#pragma warning restore xUnit1045
    public async Task GivenAnyFaultedTask_WhenOrElse_ThenShouldPropagateException(Promise<int> promise)
    {
        var sut = promise.AsTask();
        var op = Substitute.For<Func<Task<Option<int>>>>();

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () =>
        {
            var t = sut.OrElse(op);
            promise.Ready();
            await t;
        });

        Assert.Equal(expected: ExpectedException, actual: thrown);
        await op.DidNotReceive().Invoke();
    }
}

namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;
using Rufus.Tests.Utils;

public partial class OptionSyntaxTests
{
    private const int SOME_VALUE = 2;
    private static readonly ApplicationException ExpectedException = new("Expected error");

    public static TheoryData<Promise<int>> NonePromises =>
    [
        Any.Completed.Promise<int>(Any.None()),
        Any.Pending.Promise<int>(Any.None()),
    ];

    public static TheoryData<Promise<int>> FaultedPromises =>
    [
        Any.Completed.Promise<int>(ExpectedException),
        Any.Pending.Promise<int>(ExpectedException),
    ];

    public static TheoryData<Promise<int>> SomePromises =>
    [
        Any.Completed.Promise<int>(Any.Some(SOME_VALUE)),
        Any.Pending.Promise<int>(Any.Some(SOME_VALUE)),
    ];

    [Fact]
    public async Task GivenAnySomeAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldInvokeFunction()
    {
        Option<int> sut = Any.Some(SOME_VALUE);
        Option<string> expected = Any.Some("Received");
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();
        fn.Invoke(SOME_VALUE).Returns(expected);

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: expected, actual: result);
        await fn.Received(1).Invoke(SOME_VALUE);
    }

    [Fact]
    public async Task GivenAnyNoneAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnNone()
    {
        Option<int> sut = Any.None();
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();

        var result = await sut.AndThen(fn);

        Assert.IsNone(result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnySomeAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldInvokeFunction()
    {
        Option<int> sut = Any.Some(SOME_VALUE);
        Option<string> expected = Any.Some("Received");
        var fn = Substitute.For<Func<int, ValueTask<Option<string>>>>();
        fn.Invoke(SOME_VALUE).Returns(expected);

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: expected, actual: result);
        await fn.Received(1).Invoke(SOME_VALUE);
    }

    [Fact]
    public async Task GivenAnyNoneAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnNone()
    {
        Option<int> sut = Any.None();
        var fn = Substitute.For<Func<int, ValueTask<Option<string>>>>();

        var result = await sut.AndThen(fn);

        Assert.IsNone(result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnySomeAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnSomeWithoutInvokingFunction()
    {
        Option<int> sut = Any.Some(SOME_VALUE);
        var op = Substitute.For<Func<Task<Option<int>>>>();

        var result = await sut.OrElse(op);

        Assert.Equal(expected: sut, actual: result);
        await op.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task GivenAnyNoneAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldInvokeFunction()
    {
        Option<int> sut = Any.None();
        Option<int> expected = Any.Some(42);
        var op = Substitute.For<Func<Task<Option<int>>>>();
        op.Invoke().Returns(expected);

        var result = await sut.OrElse(op);

        Assert.Equal(expected: expected, actual: result);
        await op.Received(1).Invoke();
    }

    [Fact]
    public async Task GivenAnySomeAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnSomeWithoutInvokingFunction()
    {
        Option<int> sut = Any.Some(SOME_VALUE);
        var op = Substitute.For<Func<ValueTask<Option<int>>>>();

        var result = await sut.OrElse(op);

        Assert.Equal(expected: sut, actual: result);
        await op.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task GivenAnyNoneAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldInvokeFunction()
    {
        Option<int> sut = Any.None();
        Option<int> expected = Any.Some(42);
        var op = Substitute.For<Func<ValueTask<Option<int>>>>();
        op.Invoke().Returns(expected);

        var result = await sut.OrElse(op);

        Assert.Equal(expected: expected, actual: result);
        await op.Received(1).Invoke();
    }

    [Fact]
    public async Task GivenAnySomeAndFailingAsyncTaskFunction_WhenAndThen_ThenShouldThrowException()
    {
        Option<int> sut = Any.Some(SOME_VALUE);
        var fn = Substitute.For<Func<int, Task<Option<string>>>>();
        fn.Invoke(SOME_VALUE).ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () => await sut.AndThen(fn));

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }

    [Fact]
    public async Task GivenAnyNoneAndFailingAsyncTaskFunction_WhenOrElse_ThenShouldThrowException()
    {
        Option<int> sut = Any.None();
        var op = Substitute.For<Func<Task<Option<int>>>>();
        op.Invoke().ThrowsAsync(ExpectedException);

        var thrown = await Assert.ThrowsAsync<ApplicationException>(async () => await sut.OrElse(op));

        Assert.Equal(expected: ExpectedException, actual: thrown);
    }
}

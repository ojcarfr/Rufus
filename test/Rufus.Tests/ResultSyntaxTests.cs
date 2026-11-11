namespace Rufus.Tests;

using Rufus.Tests.Utils;

public partial class ResultSyntaxTests
{
    private const string EXPECTED_ERROR = "Expected error";
    private const int OK_VALUE = 2;
    private static readonly ApplicationException ExpectedException = new(EXPECTED_ERROR);

    public static TheoryData<Promise<int, string>> ErrorPromises =>
    [
        Any.Completed.Promise<int, string>(Any.Error(EXPECTED_ERROR)),
        Any.Pending.Promise<int, string>(Any.Error(EXPECTED_ERROR)),
    ];

    public static TheoryData<Promise<int, string>> FaultedPromises =>
    [
        Any.Completed.Promise<int, string>(ExpectedException), Any.Pending.Promise<int, string>(ExpectedException),
    ];

    public static TheoryData<Promise<int, string>> SucceedPromises =>
    [
        Any.Completed.Promise<int, string>(Any.Ok(OK_VALUE)), Any.Pending.Promise<int, string>(Any.Ok(OK_VALUE)),
    ];

    [Fact]
    public void GivenAnOkValue_WhenCastToGenericResult_ThenShouldMatchOkValue()
    {
        Result<int, string> sut = Any.Ok(42);

        var result = sut switch
        {
            Result.Ok<int>(var value) => value,
            _ => 0,
        };

        Assert.Equal(expected: 42, actual: result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenCastToGenericResult_ThenShouldMatchErrorValue()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut switch
        {
            Result.Error<string>(var value) => value,
            _ => null,
        };

        Assert.Equal(expected: "Expected error", actual: result);
    }

    [Fact]
    public void GivenAnOkValue_WhenCastToGenericResult_ThenShouldMatchContravariantOkValue()
    {
        Result<object, string> sut = Any.Ok<object>("ok");

        var result = sut switch
        {
            Result.Ok<object>(List<string> _) => false,
            Result.Ok<object>(string _) => true,
            _ => false,
        };

        Assert.True(result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenCastToGenericResult_ThenShouldMatchContravariantErrorValue()
    {
        Result<int, object> sut = Any.Error<object>("Expected error");

        var result = sut switch
        {
            Result.Error<object>(Exception _) => false,
            Result.Error<object>(string _) => true,
            _ => false,
        };

        Assert.True(result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenAndThen_ThenShouldInvokeBoundFunction()
    {
        Result<int, string> sut = Any.Ok(2);
        var fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(Arg.Any<int>()).Returns(Any.Ok("Received"));

        var result = sut.AndThen(fn);

        Assert.Equal(expected: Any.Ok("Received"), actual: result);
        fn.Received().Invoke(2);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAndThen_ThenShouldNotInvokeBoundFunction()
    {
        Result<int, string> sut = Any.Error("Expected error");
        var fn = Substitute.For<Func<int, Result<string, string>>>();

        var result = sut.AndThen(fn);

        Assert.Equal(expected: Any.Error("Expected error"), actual: result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, int> sut = Any.Ok(2);
        var op = Substitute.For<Func<int, Result<int, string>>>();

        var result = sut.OrElse(op);

        Assert.Equal(expected: Any.Ok(2), actual: result);
        op.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnyErrorResult_WhenOrElse_ThenShouldReturnBoundFunctionResult()
    {
        Result<int, int> sut = Any.Error(2);
        Result<int, string> expected = Any.Ok(9);
        var op = Substitute.For<Func<int, Result<int, string>>>();
        op.Invoke(Arg.Any<int>()).Returns(expected);

        var result = sut.OrElse(op);

        Assert.Equal(expected: expected, actual: result);
        op.Received().Invoke(2);
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Any.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: Any.Error(EXPECTED_ERROR), actual: result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction()
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        Result<int, string> sut = Any.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        var result = await sut.OrElse(fn);

        Assert.Equal(expected: expected, actual: result);
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Any.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: Any.Error(EXPECTED_ERROR), actual: result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction()
    {
        Result<int, int> expected = Any.Ok(OK_VALUE);
        Result<int, string> sut = Any.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        var result = await sut.OrElse(fn);

        Assert.Equal(expected: expected, actual: result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Any.Ok(OK_VALUE);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Any.Error(EXPECTED_ERROR));

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: Any.Error(EXPECTED_ERROR), actual: result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(OK_VALUE);
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();

        var result = await sut.OrElse(fn);

        Assert.Equal(expected: Any.Ok(OK_VALUE), actual: result);
        await fn.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Any.Ok(OK_VALUE);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Any.Error(EXPECTED_ERROR));

        var result = await sut.AndThen(fn);

        Assert.Equal(expected: Any.Error(EXPECTED_ERROR), actual: result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(OK_VALUE);
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();

        var result = await sut.OrElse(fn);

        Assert.Equal(expected: Any.Ok(OK_VALUE), actual: result);
        await fn.DidNotReceive().Invoke(Arg.Any<string>());
    }
}
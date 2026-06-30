using Rufus.Tests.Utils;

namespace Rufus.Tests;

public partial class ResultSyntaxTests
{
    private const string EXPECTED_ERROR = "Expected error";
    private const int OK_VALUE = 2;
    private static readonly ApplicationException ExpectedException = new(EXPECTED_ERROR);

    public static TheoryData<Promise<int, string>> ErrorPromises =>
    [
        Any.Completed.Promise<int, string>(Result.Error(EXPECTED_ERROR)),
        Any.Pending.Promise<int, string>(Result.Error(EXPECTED_ERROR)),
    ];

    public static TheoryData<Promise<int, string>> FaultedPromises =>
    [
        Any.Completed.Promise<int, string>(ExpectedException),
        Any.Pending.Promise<int, string>(ExpectedException),
    ];

    public static TheoryData<Promise<int, string>> SucceedPromises =>
    [
        Any.Completed.Promise<int, string>(Result.Ok(OK_VALUE)),
        Any.Pending.Promise<int, string>(Result.Ok(OK_VALUE)),
    ];

    [Fact]
    public void GivenAnOkValue_WhenCastToGenericResult_ThenShouldMatchOkValue()
    {
        Result<int, string> sut = Result.Ok(42);

        int result = sut switch
        {
            Ok<int>(var value) => value,
            _ => default,
        };

        Assert.Equal(42, result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenCastToGenericResult_ThenShouldMatchErrorValue()
    {
        Result<int, string> sut = Result.Error("Expected error");

        string? result = sut switch
        {
            Error<string>(var value) => value,
            _ => default,
        };

        Assert.Equal("Expected error", result);
    }

    [Fact]
    public void GivenAnOkValue_WhenCastToGenericResult_ThenShouldMatchContravariantOkValue()
    {
        Result<object, string> sut = Result.Ok<object>("ok");

        bool result = sut switch
        {
            Ok<object>(List<string> _) => false,
            Ok<object>(string _) => true,
            _ => false,
        };

        Assert.True(result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenCastToGenericResult_ThenShouldMatchContravariantErrorValue()
    {
        Result<int, object> sut = Result.Error<object>("Expected error");

        bool result = sut switch
        {
            Error<object>(Exception _) => false,
            Error<object>(string _) => true,
            _ => false,
        };

        Assert.True(result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenAndThen_ThenShouldInvokeBoundFunction()
    {
        Result<int, string> sut = Result.Ok(2);
        Func<int, Result<string, string>> fn = Substitute.For<Func<int, Result<string, string>>>();
        fn.Invoke(Arg.Any<int>()).Returns(Result.Ok("Received"));

        Result<string, string> result = sut.AndThen(fn);

        Assert.Equal(Result.Ok("Received"), result);
        fn.Received().Invoke(2);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAndThen_ThenShouldNotInvokeBoundFunction()
    {
        Result<int, string> sut = Result.Error("Expected error");
        Func<int, Result<string, string>> fn = Substitute.For<Func<int, Result<string, string>>>();

        Result<string, string> result = sut.AndThen(fn);

        Assert.Equal(Result.Error("Expected error"), result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, int> sut = Result.Ok(2);
        Func<int, Result<int, string>> op = Substitute.For<Func<int, Result<int, string>>>();

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(Result.Ok(2), result);
        op.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnyErrorResult_WhenOrElse_ThenShouldReturnBoundFunctionResult()
    {
        Result<int, int> sut = Result.Error(2);
        Result<int, string> expected = Result.Ok(9);
        Func<int, Result<int, string>> op = Substitute.For<Func<int, Result<int, string>>>();
        op.Invoke(Arg.Any<int>()).Returns(expected);

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(expected, result);
        op.Received().Invoke(2);
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction()
    {
        Result<int, int> expected = Result.Ok(OK_VALUE);
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        Result<int, int> result = await sut.OrElse(fn);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
        await fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public async Task GivenAnyErrorResultAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnResultFromFunction()
    {
        Result<int, int> expected = Result.Ok(OK_VALUE);
        Result<int, string> sut = Result.Error(EXPECTED_ERROR);
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();
        fn.Invoke(EXPECTED_ERROR).Returns(expected);

        Result<int, int> result = await sut.OrElse(fn);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Result.Ok(OK_VALUE);
        var fn = Substitute.For<Func<int, Task<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Result.Error(EXPECTED_ERROR));

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncTaskFunction_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(OK_VALUE);
        var fn = Substitute.For<Func<string, Task<Result<int, int>>>>();

        Result<int, int> result = await sut.OrElse(fn);

        Assert.Equal(Result.Ok(OK_VALUE), result);
        await fn.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncValueTaskFunction_WhenAndThen_ThenShouldReturnResultFromFunction()
    {
        Result<int, string> sut = Result.Ok(OK_VALUE);
        var fn = Substitute.For<Func<int, ValueTask<Result<string, string>>>>();
        fn.Invoke(OK_VALUE).Returns(Result.Error(EXPECTED_ERROR));

        Result<string, string> result = await sut.AndThen(fn);

        Assert.Equal(Result.Error(EXPECTED_ERROR), result);
    }

    [Fact]
    public async Task GivenAnyOkResultAndAnyAsyncValueTaskFunction_WhenOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(OK_VALUE);
        var fn = Substitute.For<Func<string, ValueTask<Result<int, int>>>>();

        Result<int, int> result = await sut.OrElse(fn);

        Assert.Equal(Result.Ok(OK_VALUE), result);
        await fn.DidNotReceive().Invoke(Arg.Any<string>());
    }
}
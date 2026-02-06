namespace Rufus.Tests;

public class ResultTests
{
    [Fact]
    public void GivenAnyOkResult_WhenAnd_AnyOk_ThenShouldReturnBoundOk()
    {
        Result<int, string> sut = Result.Ok(2);
        Result<string, string> expected = Result.Ok("different ok");

        Result<string, string> result = sut.And(expected);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenAnd_AnyError_ThenShouldReturnBoundError()
    {
        Result<int, string> sut = Result.Ok(2);
        Result<string, string> expected = Result.Error("late error");

        Result<string, string> result = sut.And(expected);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAnd_AnyOk_ThenShouldReturnSourceError()
    {
        Result<int, string> sut = Result.Error("early error");
        Result<string, string> expected = Result.Ok("foo");

        Result<string, string> result = sut.And(expected);

        Assert.Equal(Result.Error("early error"), result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAnd_AnyError_ThenShouldReturnSourceError()
    {
        Result<int, string> sut = Result.Error("not a 2");
        Result<string, string> expected = Result.Error("late error");

        Result<string, string> result = sut.And(expected);

        Assert.Equal(Result.Error("not a 2"), result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyBindableFunction_WhenAndThen_ThenBoundFunctionShouldBeInvoked()
    {
        const int OK_VALUE = 42;
        Result<int, string> sut = Result.Ok(OK_VALUE);
        Result<string, string> expected = Result.Ok("Bound value");
        Func<int, Result<string, string>> bindableFunction = Substitute.For<Func<int, Result<string, string>>>();
        bindableFunction.Invoke(OK_VALUE).Returns(expected);

        Result<string, string> result = sut.AndThen(bindableFunction);

        Assert.Equal(expected, result);
        bindableFunction.Received().Invoke(OK_VALUE);
    }

    [Fact]
    public void GivenAnErrorResultAndAnyBindableFunction_WhenAndThen_ThenBoundFunctionShouldNotBeInvoked()
    {
        Result<int, string> sut = Result.Error("Expected error");
        Func<int, Result<string, string>> bindableFunction = Substitute.For<Func<int, Result<string, string>>>();

        Result<string, string> result = sut.AndThen(bindableFunction);

        Assert.Equal(Result.Error("Expected error"), result);
        bindableFunction.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOk_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Result.Ok(100);

        Assert.True(sut.IsOk);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsOk_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Error("Expected error");

        Assert.False(sut.IsOk);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsError_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Ok(100);

        Assert.False(sut.IsError);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsError_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Result.Error("Expected error");

        Assert.True(sut.IsError);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsErrorAndPredicateMatches_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Ok(50);

        bool result = sut.IsErrorAnd(_ => true);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsErrorAndPredicateMatches_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Result.Error("Expected error");

        bool result = sut.IsErrorAnd(_ => true);

        Assert.True(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsErrorAndPredicateDoesNotMatch_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Error("Expected error");

        bool result = sut.IsErrorAnd(_ => false);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOkAndPredicateMatches_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Result.Ok(50);

        bool result = sut.IsOkAnd(value => value > 0);

        Assert.True(result);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOkAndPredicateDoesNotMatch_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Ok(-10);

        bool result = sut.IsOkAnd(value => value > 0);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsOkAndPredicateMatches_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Result.Error("Expected error");

        bool result = sut.IsOkAnd(_ => true);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyCallbackFunction_WhenInspect_ThenFunctionShouldBeInvoked()
    {
        Action<int> callback = Substitute.For<Action<int>>();
        Result<int, string> sut = Result.Ok(4);

        sut.Inspect(callback);

        callback.Received().Invoke(4);
    }

    [Fact]
    public void GivenAnErrorResultAndAnyCallbackFunction_WhenInspect_ThenFunctionShouldNotBeInvoked()
    {
        Action<int> callback = Substitute.For<Action<int>>();
        Result<int, string> sut = Result.Error(string.Empty);

        sut.Inspect(callback);

        callback.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnOkResultAndAnyCallbackFunction_WhenInspectError_ThenFunctionShouldNotBeInvoked()
    {
        Action<string> callback = Substitute.For<Action<string>>();
        Result<int, string> sut = Result.Ok(4);

        sut.InspectError(callback);

        callback.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public void GivenAnErrorResultAndAnyCallbackFunction_WhenInspectError_ThenFunctionShouldNotBeInvoked()
    {
        Action<string> callback = Substitute.For<Action<string>>();
        Result<int, string> sut = Result.Error("Expected error");

        sut.InspectError(callback);

        callback.Received().Invoke("Expected error");
    }

    [Fact]
    public void GivenAnOkResult_WhenMap_ThenShouldTransformValue()
    {
        Result<string, string> sut = Result.Ok("1,2,3,4,5");

        Result<int, string> result =
            sut.Map(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Sum());

        Assert.Equal(15, Assert.IsType<Result<int, string>.Ok>(result).Value);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMap_ThenShouldReturnErrorUnchanged()
    {
        Result<string, string> sut = Result.Error("Expected error");

        Result<int, string> result = sut.Map(_ => 0);

        Assert.Equal("Expected error", Assert.IsType<Result<int, string>.Error>(result).Value);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapOr_ThenShouldMapValue()
    {
        Result<string, string> sut = Result.Ok("foo");

        int result = sut.MapOr(x => x.Length, 42);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapOr_ThenShouldReturnDefaultValue()
    {
        Result<string, string> sut = Result.Error("bar");

        int result = sut.MapOr(x => x.Length, 42);

        Assert.Equal(42, result);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapOrElse_ThenShouldMapValue()
    {
        Result<string, string> sut = Result.Ok("foo");

        int result = sut.MapOrElse(x => x.Length, fallback: _ => 42);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapOrElse_ThenShouldReturnDefaultValue()
    {
        Result<string, string> sut = Result.Error("bar");

        int result = sut.MapOrElse(x => x.Length, _ => 42);

        Assert.Equal(42, result);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapError_ThenShouldReturnOkUnchanged()
    {
        Result<int, int> sut = Result.Ok(2);

        Result<int, string> result = sut.MapError(static err => $"error code: {err}");

        Assert.Equal(Result.Ok(2), result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapError_ThenShouldTransformErrorValue()
    {
        Result<int, int> sut = Result.Error(13);

        Result<int, string> result = sut.MapError(static err => $"error code: {err}");

        Assert.Equal(Result.Error("error code: 13"), result);
    }

    [Fact]
    public void GivenAnOkResult_WhenOr_ThenShouldReturnOk()
    {
        Result<int, int> sut = Result.Ok(2);
        Result<int, string> other = Result.Error("disregarded");

        Result<int, string> result = sut.Or(other);

        Assert.Equal(Result.Ok(2), result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenOr_ThenShouldReturnOtherResult()
    {
        Result<int, int> sut = Result.Error(2);
        Result<int, string> other = Result.Error("Expected error");

        Result<int, string> result = sut.Or(other);

        Assert.Equal(other, result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyBindableFunction_WhenOrElse_ThenBoundFunctionShouldNotBeInvoked()
    {
        ResultSyntax.OkValue<int> expected = Result.Ok(2);
        Result<int, int> sut = Result.Ok(expected.Value);
        Func<int, Result<int, string>>? op = Substitute.For<Func<int, Result<int, string>>>();

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(expected, result);
        op.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnErrorResultAndAnyBindableFunction_WhenOrElse_ThenBoundFunctionShouldBeInvoked()
    {
        ResultSyntax.OkValue<int> expected = Result.Ok(50);
        Result<int, int> sut = Result.Error(2);
        Func<int, Result<int, string>> op = Substitute.For<Func<int, Result<int, string>>>();
        op.Invoke(Arg.Any<int>()).Returns(expected);

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(expected, result);
        op.Received().Invoke(2);
    }
}
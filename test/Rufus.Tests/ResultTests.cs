namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;

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
        Result.Values.Ok<int> expected = Result.Ok(2);
        Result<int, int> sut = Result.Ok(expected.Value);
        Func<int, Result<int, string>>? op = Substitute.For<Func<int, Result<int, string>>>();

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(expected, result);
        op.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnErrorResultAndAnyBindableFunction_WhenOrElse_ThenBoundFunctionShouldBeInvoked()
    {
        Result.Values.Ok<int> expected = Result.Ok(50);
        Result<int, int> sut = Result.Error(2);
        Func<int, Result<int, string>> op = Substitute.For<Func<int, Result<int, string>>>();
        op.Invoke(Arg.Any<int>()).Returns(expected);

        Result<int, string> result = sut.OrElse(op);

        Assert.Equal(expected, result);
        op.Received().Invoke(2);
    }

    [Fact]
    public void GivenAnyOkResultWithAnyNestedResult_WhenFlatten_ThenShouldReturnNestedResult()
    {
        Result<string, int> expected = Result.Error(6);
        Result<Result<string, int>, int> sut = Result.Ok(expected);

        Result<string, int> result = sut.Flatten();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenFlatten_ThenShouldReturnSameError()
    {
        Result<Result<string, int>, int> sut = Result.Error(6);

        Result<string, int> result = sut.Flatten();

        Assert.Equal(Result.Error(6), result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrap_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(2);

        int result = sut.Unwrap();

        Assert.Equal(2, result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenUnwrap_ThenShouldThrowAnInvalidException()
    {
        Result<int, string> sut = Result.Error("Expected error");

        InvalidOperationException thrownException = Assert.Throws<InvalidOperationException>(() => sut.Unwrap());

        Assert.Equal("Expected error", thrownException.Message);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyExceptionFactory_WhenUnwrap_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(2);

        int result = sut.Unwrap<ApplicationException>(_ => throw new ApplicationException("Disregarded"));

        Assert.Equal(2, result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyExceptionFactory_WhenUnwrap_ThenShouldThrowBuiltException()
    {
        var expectedException = new ApplicationException("Thrown: Expected error");
        Func<string, ApplicationException> exceptionFactory = Substitute.For<Func<string, ApplicationException>>();
        exceptionFactory.Invoke(Arg.Any<string>()).Returns(expectedException);
        Result<int, string> sut = Result.Error("Expected error");

        ApplicationException receivedException =
            Assert.Throws<ApplicationException>(() => sut.Unwrap(exceptionFactory));

        Assert.Equal(expectedException, receivedException);
        exceptionFactory.Received().Invoke("Expected error");
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrapError_ThenShouldThrowInvalidOperationException()
    {
        Result<int, string> sut = Result.Ok(2);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => sut.UnwrapError());

        Assert.Equal("2", exception.Message);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenUnwrapError_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Result.Error("Expected error");

        string result = sut.UnwrapError();

        Assert.Equal("Expected error", result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyExceptionFactory_WhenUnwrapError_ThenShouldThrowExceptionBuiltByFactory()
    {
        var expectedException = new ApplicationException("Thrown: 2");
        Result<int, string> sut = Result.Ok(2);
        Func<int, ApplicationException> exceptionFactory = Substitute.For<Func<int, ApplicationException>>();
        exceptionFactory.Invoke(2).Throws(expectedException);

        ApplicationException exception = Assert.Throws<ApplicationException>(() => sut.UnwrapError(exceptionFactory));

        Assert.Equal(expectedException, exception);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyExceptionFactory_WhenUnwrapError_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Result.Error("Expected error");

        string result = sut.UnwrapError<ApplicationException>(_ => new ApplicationException());

        Assert.Equal("Expected error", result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyDefaultValue_WhenUnwrapOr_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(9);

        int result = sut.UnwrapOr(2);

        Assert.Equal(9, result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyDefaultValue_WhenUnwrapOr_ThenShouldReturnDefaultValue()
    {
        Result<int, string> sut = Result.Error("error");

        int result = sut.UnwrapOr(2);

        Assert.Equal(2, result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrapOrDefault_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(2);

        int result = sut.UnwrapOrDefault();

        Assert.Equal(2, result);
    }

    [Fact]
    public void GivenAnyErrorResultOfAnyReferenceType_WhenUnwrapOrDefault_ThenShouldReturnNull()
    {
        Result<object, string> sut = Result.Error("error");

        object? result = sut.UnwrapOrDefault();

        Assert.Null(result);
    }

    [Fact]
    public void GivenAnyErrorResultOfAnyValueType_WhenUnwrapOrDefault_ThenShouldReturnDefaultValueType()
    {
        Result<int, string> sut = Result.Error("error");

        int result = sut.UnwrapOrDefault();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyOptionalFunction_WhenUnwrapOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Result.Ok(2);

        int result = sut.UnwrapOrElse(_ => int.MaxValue);

        Assert.Equal(2, result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyOptionalFunction_WhenUnwrapOrElse_ThenShouldReturnOptionalFunctionResult()
    {
        Result<int, string> sut = Result.Error("Expected error");
        Func<string, int> op = Substitute.For<Func<string, int>>();
        op.Invoke("Expected error").Returns(int.MaxValue);

        int result = sut.UnwrapOrElse(op);

        Assert.Equal(int.MaxValue, result);
    }
}
namespace Rufus.Tests;

using NSubstitute.ExceptionExtensions;

public class ResultTests
{
    [Fact]
    public void GivenAnyOkResult_WhenAnd_AnyOk_ThenShouldReturnBoundOk()
    {
        Result<int, string> sut = Any.Ok(2);
        Result<string, string> expected = Any.Ok("different ok");

        var result = sut.And(expected);

        Assert.Equal(expected: expected, actual: result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenAnd_AnyError_ThenShouldReturnBoundError()
    {
        Result<int, string> sut = Any.Ok(2);
        Result<string, string> expected = Any.Error("late error");

        var result = sut.And(expected);

        Assert.Equal(expected: expected, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAnd_AnyOk_ThenShouldReturnSourceError()
    {
        Result<int, string> sut = Any.Error("early error");
        Result<string, string> expected = Any.Ok("foo");

        var result = sut.And(expected);

        Assert.Equal(expected: Any.Error("early error"), actual: result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenAnd_AnyError_ThenShouldReturnSourceError()
    {
        Result<int, string> sut = Any.Error("not a 2");
        Result<string, string> expected = Any.Error("late error");

        var result = sut.And(expected);

        Assert.Equal(expected: Any.Error("not a 2"), actual: result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyBindableFunction_WhenAndThen_ThenBoundFunctionShouldBeInvoked()
    {
        const int OK_VALUE = 42;
        Result<int, string> sut = Any.Ok(OK_VALUE);
        Result<string, string> expected = Any.Ok("Bound value");
        var bindableFunction = Substitute.For<Func<int, Result<string, string>>>();
        bindableFunction.Invoke(OK_VALUE).Returns(expected);

        var result = sut.AndThen(bindableFunction);

        Assert.Equal(expected: expected, actual: result);
        bindableFunction.Received().Invoke(OK_VALUE);
    }

    [Fact]
    public void GivenAnErrorResultAndAnyBindableFunction_WhenAndThen_ThenBoundFunctionShouldNotBeInvoked()
    {
        Result<int, string> sut = Any.Error("Expected error");
        var bindableFunction = Substitute.For<Func<int, Result<string, string>>>();

        var result = sut.AndThen(bindableFunction);

        Assert.Equal(expected: Any.Error("Expected error"), actual: result);
        bindableFunction.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOk_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Any.Ok(100);

        Assert.True(sut.IsOk);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsOk_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Error("Expected error");

        Assert.False(sut.IsOk);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsError_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Ok(100);

        Assert.False(sut.IsError);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsError_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Any.Error("Expected error");

        Assert.True(sut.IsError);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsErrorAndPredicateMatches_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Ok(50);

        var result = sut.IsErrorAnd(_ => true);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsErrorAndPredicateMatches_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut.IsErrorAnd(_ => true);

        Assert.True(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsErrorAndPredicateDoesNotMatch_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut.IsErrorAnd(_ => false);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOkAndPredicateMatches_ThenShouldReturnTrue()
    {
        Result<int, string> sut = Any.Ok(50);

        var result = sut.IsOkAnd(value => value > 0);

        Assert.True(result);
    }

    [Fact]
    public void GivenAnOkResult_WhenCheckingIsOkAndPredicateDoesNotMatch_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Ok(-10);

        var result = sut.IsOkAnd(value => value > 0);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenCheckingIsOkAndPredicateMatches_ThenShouldReturnFalse()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut.IsOkAnd(_ => true);

        Assert.False(result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyCallbackFunction_WhenInspect_ThenFunctionShouldBeInvoked()
    {
        var callback = Substitute.For<Action<int>>();
        Result<int, string> sut = Any.Ok(4);

        sut.Inspect(callback);

        callback.Received().Invoke(4);
    }

    [Fact]
    public void GivenAnErrorResultAndAnyCallbackFunction_WhenInspect_ThenFunctionShouldNotBeInvoked()
    {
        var callback = Substitute.For<Action<int>>();
        Result<int, string> sut = Any.Error(string.Empty);

        sut.Inspect(callback);

        callback.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnOkResultAndAnyCallbackFunction_WhenInspectError_ThenFunctionShouldNotBeInvoked()
    {
        var callback = Substitute.For<Action<string>>();
        Result<int, string> sut = Any.Ok(4);

        sut.InspectError(callback);

        callback.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public void GivenAnErrorResultAndAnyCallbackFunction_WhenInspectError_ThenFunctionShouldNotBeInvoked()
    {
        var callback = Substitute.For<Action<string>>();
        Result<int, string> sut = Any.Error("Expected error");

        sut.InspectError(callback);

        callback.Received().Invoke("Expected error");
    }

    [Fact]
    public void GivenAnOkResult_WhenMap_ThenShouldTransformValue()
    {
        Result<string, string> sut = Any.Ok("1,2,3,4,5");

        var result =
            sut.Map(x => x.
                Split(separator: ',', options: StringSplitOptions.RemoveEmptyEntries).
                Select(int.Parse).
                Sum());

        Assert.Equal(expected: 15, actual: Assert.IsType<Result<int, string>.Ok>(result).Value);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMap_ThenShouldReturnErrorUnchanged()
    {
        Result<string, string> sut = Any.Error("Expected error");

        var result = sut.Map(_ => 0);

        Assert.Equal(expected: "Expected error", actual: Assert.IsType<Result<int, string>.Error>(result).Value);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapOr_ThenShouldMapValue()
    {
        Result<string, string> sut = Any.Ok("foo");

        var result = sut.MapOr(map: x => x.Length, @default: 42);

        Assert.Equal(expected: 3, actual: result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapOr_ThenShouldReturnDefaultValue()
    {
        Result<string, string> sut = Any.Error("bar");

        var result = sut.MapOr(map: x => x.Length, @default: 42);

        Assert.Equal(expected: 42, actual: result);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapOrElse_ThenShouldMapValue()
    {
        Result<string, string> sut = Any.Ok("foo");

        var result = sut.MapOrElse(map: x => x.Length, fallback: _ => 42);

        Assert.Equal(expected: 3, actual: result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapOrElse_ThenShouldReturnDefaultValue()
    {
        Result<string, string> sut = Any.Error("bar");

        var result = sut.MapOrElse(map: x => x.Length, fallback: _ => 42);

        Assert.Equal(expected: 42, actual: result);
    }

    [Fact]
    public void GivenAnOkResult_WhenMapError_ThenShouldReturnOkUnchanged()
    {
        Result<int, int> sut = Any.Ok(2);

        var result = sut.MapError(static err => $"error code: {err}");

        Assert.Equal(expected: Any.Ok(2), actual: result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenMapError_ThenShouldTransformErrorValue()
    {
        Result<int, int> sut = Any.Error(13);

        var result = sut.MapError(static err => $"error code: {err}");

        Assert.Equal(expected: Any.Error("error code: 13"), actual: result);
    }

    [Fact]
    public void GivenAnOkResult_WhenOr_ThenShouldReturnOk()
    {
        Result<int, int> sut = Any.Ok(2);
        Result<int, string> other = Any.Error("disregarded");

        var result = sut.Or(other);

        Assert.Equal(expected: Any.Ok(2), actual: result);
    }

    [Fact]
    public void GivenAnErrorResult_WhenOr_ThenShouldReturnOtherResult()
    {
        Result<int, int> sut = Any.Error(2);
        Result<int, string> other = Any.Error("Expected error");

        var result = sut.Or(other);

        Assert.Equal(expected: other, actual: result);
    }

    [Fact]
    public void GivenAnOkResultAndAnyBindableFunction_WhenOrElse_ThenBoundFunctionShouldNotBeInvoked()
    {
        var expected = Any.Ok(2);
        Result<int, int> sut = Any.Ok(expected.Value);
        var op = Substitute.For<Func<int, Result<int, string>>>();

        var result = sut.OrElse(op);

        Assert.Equal(expected: expected, actual: result);
        op.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnErrorResultAndAnyBindableFunction_WhenOrElse_ThenBoundFunctionShouldBeInvoked()
    {
        var expected = Any.Ok(50);
        Result<int, int> sut = Any.Error(2);
        var op = Substitute.For<Func<int, Result<int, string>>>();
        op.Invoke(Arg.Any<int>()).Returns(expected);

        var result = sut.OrElse(op);

        Assert.Equal(expected: expected, actual: result);
        op.Received().Invoke(2);
    }

    [Fact]
    public void GivenAnyOkResultWithAnyNestedResult_WhenFlatten_ThenShouldReturnNestedResult()
    {
        Result<string, int> expected = Any.Error(6);
        Result<Result<string, int>, int> sut = Any.Ok(expected);

        var result = sut.Flatten();

        Assert.Equal(expected: expected, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenFlatten_ThenShouldReturnSameError()
    {
        Result<Result<string, int>, int> sut = Any.Error(6);

        var result = sut.Flatten();

        Assert.Equal(expected: Any.Error(6), actual: result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrap_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(2);

        var result = sut.Unwrap();

        Assert.Equal(expected: 2, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenUnwrap_ThenShouldThrowAnInvalidException()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var thrownException = Assert.Throws<InvalidOperationException>(() => sut.Unwrap());

        Assert.Equal(expected: "Expected error", actual: thrownException.Message);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyExceptionFactory_WhenUnwrap_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(2);

        var result = sut.Unwrap<ApplicationException>(_ => throw new ApplicationException("Disregarded"));

        Assert.Equal(expected: 2, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyExceptionFactory_WhenUnwrap_ThenShouldThrowBuiltException()
    {
        var expectedException = new ApplicationException("Thrown: Expected error");
        var exceptionFactory = Substitute.For<Func<string, ApplicationException>>();
        exceptionFactory.Invoke(Arg.Any<string>()).Returns(expectedException);
        Result<int, string> sut = Any.Error("Expected error");

        var receivedException =
            Assert.Throws<ApplicationException>(() => sut.Unwrap(exceptionFactory));

        Assert.Equal(expected: expectedException, actual: receivedException);
        exceptionFactory.Received().Invoke("Expected error");
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrapError_ThenShouldThrowInvalidOperationException()
    {
        Result<int, string> sut = Any.Ok(2);

        var exception = Assert.Throws<InvalidOperationException>(() => sut.UnwrapError());

        Assert.Equal(expected: "2", actual: exception.Message);
    }

    [Fact]
    public void GivenAnyErrorResult_WhenUnwrapError_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut.UnwrapError();

        Assert.Equal(expected: "Expected error", actual: result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyExceptionFactory_WhenUnwrapError_ThenShouldThrowExceptionBuiltByFactory()
    {
        var expectedException = new ApplicationException("Thrown: 2");
        Result<int, string> sut = Any.Ok(2);
        var exceptionFactory = Substitute.For<Func<int, ApplicationException>>();
        exceptionFactory.Invoke(2).Throws(expectedException);

        var exception = Assert.Throws<ApplicationException>(() => sut.UnwrapError(exceptionFactory));

        Assert.Equal(expected: expectedException, actual: exception);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyExceptionFactory_WhenUnwrapError_ThenShouldReturnErrorValue()
    {
        Result<int, string> sut = Any.Error("Expected error");

        var result = sut.UnwrapError<ApplicationException>(_ => new ApplicationException());

        Assert.Equal(expected: "Expected error", actual: result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyDefaultValue_WhenUnwrapOr_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(9);

        var result = sut.UnwrapOr(2);

        Assert.Equal(expected: 9, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyDefaultValue_WhenUnwrapOr_ThenShouldReturnDefaultValue()
    {
        Result<int, string> sut = Any.Error("error");

        var result = sut.UnwrapOr(2);

        Assert.Equal(expected: 2, actual: result);
    }

    [Fact]
    public void GivenAnyOkResult_WhenUnwrapOrDefault_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(2);

        var result = sut.UnwrapOrDefault();

        Assert.Equal(expected: 2, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResultOfAnyReferenceType_WhenUnwrapOrDefault_ThenShouldReturnNull()
    {
        Result<object, string> sut = Any.Error("error");

        var result = sut.UnwrapOrDefault();

        Assert.Null(result);
    }

    [Fact]
    public void GivenAnyErrorResultOfAnyValueType_WhenUnwrapOrDefault_ThenShouldReturnDefaultValueType()
    {
        Result<int, string> sut = Any.Error("error");

        var result = sut.UnwrapOrDefault();

        Assert.Equal(expected: 0, actual: result);
    }

    [Fact]
    public void GivenAnyOkResultAndAnyOptionalFunction_WhenUnwrapOrElse_ThenShouldReturnOkValue()
    {
        Result<int, string> sut = Any.Ok(2);

        var result = sut.UnwrapOrElse(_ => int.MaxValue);

        Assert.Equal(expected: 2, actual: result);
    }

    [Fact]
    public void GivenAnyErrorResultAndAnyOptionalFunction_WhenUnwrapOrElse_ThenShouldReturnOptionalFunctionResult()
    {
        Result<int, string> sut = Any.Error("Expected error");
        var op = Substitute.For<Func<string, int>>();
        op.Invoke("Expected error").Returns(int.MaxValue);

        var result = sut.UnwrapOrElse(op);

        Assert.Equal(expected: int.MaxValue, actual: result);
    }
}
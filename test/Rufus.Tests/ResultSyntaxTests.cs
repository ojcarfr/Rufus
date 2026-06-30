namespace Rufus.Tests;

public class ResultSyntaxTests
{
    [Fact]
    public void GivenAnOkValue_WhenDeconstruct_ThenShouldAssignUnderlyingValue()
    {
        Result sut = Result.Ok(5);

        if(sut is not Result.Ok<int>(var result))
        {
            Assert.Fail("Result is not OK");

            return;
        }

        Assert.Equal(5, result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenDeconstruct_ThenShouldAssignUnderlyingValue()
    {
        Result sut = Result.Error("Expected error");

        if(sut is not Result.Error<string>(var result))
        {
            Assert.Fail("Result is not Error");

            return;
        }

        Assert.Equal("Expected error", result);
    }

    [Fact]
    public void GivenAnOkValue_WhenSwitch_ThenShouldMatchCovariantUnderlyingValue()
    {
        Result sut = Result.Ok("OK");

        object? result = sut switch
        {
            Result.Ok<object>(var value) => value,
            _ => default,
        };

        Assert.Equal("OK", result);
    }

    [Fact]
    public void GivenAnErrorValue_WhenSwitch_ThenShouldMatchCovariantUnderlyingValue()
    {
        Result sut = Result.Error("Expected error");

        object? result = sut switch
        {
            Result.Error<object>(var value) => value,
            _ => default,
        };

        Assert.Equal("Expected error", result);
    }

    [Fact]
    public void GivenAnOkValue_WhenCastToGenericResult_ThenShouldMatchOkValue()
    {
        Result<int, string> sut = Result.Ok(42);

        int result = sut switch
        {
            Result.Ok<int>(var value) => value,
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
            Result.Error<string>(var value) => value,
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
            Result.Ok<object>(List<string> _) => false,
            Result.Ok<object>(string _) => true,
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
            Result.Error<object>(Exception _) => false,
            Result.Error<object>(string _) => true,
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
}
namespace Rufus.Tests;

public class ResultTests
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
}
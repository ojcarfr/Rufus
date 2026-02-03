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
    public void GivenAnyResultValue_WhenAndThen_ThenShouldReturnBoundFunctionResult()
    {
        static Result<string, string> SqThenToString(int value)
        {
            checked
            {
                try
                {
                    return Result.Ok((value * value).ToString());
                }
                catch(OverflowException)
                {
                    return Result.Error("overflowed");
                }
            }
        }

        Assert.Equal(Result.Ok(4.ToString()), Result.Ok(2).AndThen(SqThenToString));
        Assert.Equal(Result.Error("overflowed"), Result.Ok(1_000_000).AndThen(SqThenToString));
        Assert.Equal(Result.Error("not a number"), Result.Error("not a number").AndThen((int x) => SqThenToString(x)));
    }
}
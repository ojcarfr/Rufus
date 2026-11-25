namespace Rufus.Tests;

using Rufus.Tests.Utils;

public class OptionTests
{
    [Fact]
    public void GivenAnyValue_WhenSome_ThenShouldReturnSomeValue()
    {
        Option<int> sut = Any.Some(5);

        Assert.Equal(5, Assert.IsSome(sut).Value);
    }

    [Fact]
    public void GivenNoValue_WhenNone_ThenShouldReturnNone()
    {
        Option<int> sut = Any.None();

        Assert.IsNone(sut);
    }

#if NET10_0_OR_GREATER
    [Fact]
    public void GivenAnyValue_WhenSome_ThenShouldCastToGenericSome()
    {
        Option<int> sut = Option.Some(5);
        Assert.Equal(5, Assert.IsSome(sut).Value);
    }

    [Fact]
    public void GivenNoValue_WhenNone_TheShouldCastToGenericNone()
    {
        Option<int> sut = Option.None();
        Assert.IsNone(sut);
    }
#endif

    [Fact]
    public void GivenAnySome_WhenIsSome_ThenShouldReturnTrue()
    {
        Option<int> sut = Any.Some(5);

        var result = sut.IsSome;

        Assert.True(result);
    }

    [Fact]
    public void GivenAnyNone_WhenIsSome_ThenShouldReturnFalse()
    {
        Option<int> sut = Any.None();
        var result = sut.IsSome;
        Assert.False(result);
    }

    [Fact]
    public void GivenAnySome_WhenIsNone_ThenShouldReturnFalse()
    {
        Option<int> sut = Any.Some(5);
        var result = sut.IsNone;
        Assert.False(result);
    }

    [Fact]
    public void GivenAnyNone_WhenIsNone_ThenShouldReturnTrue()
    {
        Option<int> sut = Any.None();
        var result = sut.IsNone;
        Assert.True(result);
    }

    [Fact]
    public void GivenAnySomeAndAnyMatchingPredicate_WhenIsSomeAnd_ThenShouldReturnTrue()
    {
        Option<int> sut = Any.Some(2);
        var result = sut.IsSomeAnd(x => x > 1);
        Assert.True(result);
    }

    [Fact]
    public void GivenAnySomeAndAnyUnmatchingPredicate_WhenIsSomeAnd_ThenShouldReturnFalse()
    {
        Option<int> sut = Any.Some(0);
        var result = sut.IsSomeAnd(x => x > 1);
        Assert.False(result);
    }

    [Fact]
    public void GivenAnyNoneAndAnyPredicate_WhenIsSomeAnd_ThenShouldReturnFalse()
    {
        Option<int> sut = Any.None();
        var result = sut.IsSomeAnd(_ => true);
        Assert.False(result);
    }

    [Fact]
    public void GivenAnySomeAndAnyMatchingPredicate_WhenIsNoneOr_ThenShouldReturnTrue()
    {
        Option<int> sut = Any.Some(2);
        var result = sut.IsNoneOr(x => x > 1);
        Assert.True(result);
    }

    [Fact]
    public void GivenAnySomeAndAnyUnmatchingPredicate_WhenIsNoneOr_ThenShouldReturnFalse()
    {
        Option<int> sut = Any.Some(0);
        var result = sut.IsNoneOr(x => x > 1);
        Assert.False(result);
    }

    [Fact]
    public void GivenAnyNoneAndAnyPredicate_WhenIsNoneOr_ThenShouldReturnTrue()
    {
        Option<int> sut = Any.None();
        var result = sut.IsNoneOr(_ => false);
        Assert.True(result);
    }
}
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
}
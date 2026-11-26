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

    [Fact]
    public void GivenAnySomeValue_WhenExpect_ThenShouldReturnValue()
    {
        Option<string> sut = Any.Some("value");
        var result = sut.Expect("disregarded");
        Assert.Equal("value", result);
    }

    [Fact]
    public void GivenAnyNone_WhenExpect_ThenShouldThrowException()
    {
        Option<string> sut = Any.None();
        var exception = Assert.Throws<InvalidOperationException>(() => sut.Expect("TEST OK"));
        Assert.Equal("TEST OK", exception.Message);
    }

    [Fact]
    public void GivenAnySome_WhenUnwrap_ThenShouldReturnValue()
    {
        Option<string> sut = Any.Some("air");
        Assert.Equal("air", sut.Unwrap());
    }

    [Fact]
    public void GivenAnyNone_WhenUnwrap_ThenShouldThrowInvalidOperationException()
    {
        Option<string> sut = Any.None();
        Assert.Throws<InvalidOperationException>(sut.Unwrap);
    }

    [Fact]
    public void GivenAnySome_WhenUnwrapOr_ThenShouldReturnValue()
    {
        Option<string> sut = Any.Some("car");
        Assert.Equal("car", sut.UnwrapOr("bike"));
    }

    [Fact]
    public void GivenAnyNone_WhenUnwrapOr_ThenShouldReturnDefault()
    {
        Option<string> sut = Any.None();
        Assert.Equal("bike", sut.UnwrapOr("bike"));
    }

    [Fact]
    public void GivenAnySome_WhenUnwrapOrElse_ThenShouldReturnValueWithoutInvokingFallback()
    {
        Option<int> sut = Any.Some(4);
        var fallback = Substitute.For<Func<int>>();

        var result = sut.UnwrapOrElse(fallback);

        Assert.Equal(4, result);
        fallback.DidNotReceive().Invoke();
    }

    [Fact]
    public void GivenAnyNone_WhenUnwrapOrElse_ThenShouldInvokeFallbackAndReturnResult()
    {
        Option<int> sut = Any.None();
        var fallback = Substitute.For<Func<int>>();
        fallback.Invoke().Returns(20);

        var result = sut.UnwrapOrElse(fallback);

        Assert.Equal(20, result);
        fallback.Received(1).Invoke();
    }

    [Fact]
    public void GivenAnySome_WhenUnwrapOrDefault_ThenShouldReturnValue()
    {
        Option<int> sut = Any.Some(12);
        Assert.Equal(12, sut.UnwrapOrDefault());
    }

    [Fact]
    public void GivenAnyNone_WhenUnwrapOrDefault_ThenShouldReturnTypeDefault()
    {
        Option<int> intNone = Any.None();
        Option<string> strNone = Any.None();

        Assert.Equal(0, intNone.UnwrapOrDefault());
        Assert.Null(strNone.UnwrapOrDefault());
    }

    [Fact]
    public void GivenAnySome_WhenInspect_ThenShouldInvokeActionAndReturnSelf()
    {
        Option<int> sut = Any.Some(42);
        var action = Substitute.For<Action<int>>();

        var result = sut.Inspect(action);

        Assert.Equal(sut, result);
        action.Received(1).Invoke(42);
    }

    [Fact]
    public void GivenAnyNone_WhenInspect_ThenShouldNotInvokeActionAndReturnSelf()
    {
        Option<int> sut = Any.None();
        var action = Substitute.For<Action<int>>();

        var result = sut.Inspect(action);

        Assert.Equal(sut, result);
        action.DidNotReceive().Invoke(Arg.Any<int>());
    }

    [Fact]
    public void GivenAnySome_WhenMap_ThenShouldReturnMappedSome()
    {
        Option<string> sut = Any.Some("Hello, World!");

        var result = sut.Map(s => s.Length);

        Assert.Equal(13, Assert.IsSome(result).Value);
    }

    [Fact]
    public void GivenAnyNone_WhenMap_ThenShouldReturnNoneWithoutInvokingMapper()
    {
        Option<string> sut = Any.None();
        var mapper = Substitute.For<Func<string, int>>();

        var result = sut.Map(mapper);

        Assert.IsNone(result);
        mapper.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public void GivenAnySome_WhenMapOr_ThenShouldReturnMappedValue()
    {
        Option<string> sut = Any.Some("foo");
        Assert.Equal(3, sut.MapOr(v => v.Length, 42));
    }

    [Fact]
    public void GivenAnyNone_WhenMapOr_ThenShouldReturnDefault()
    {
        Option<string> sut = Any.None();
        Assert.Equal(42, sut.MapOr(v => v.Length, 42));
    }

    [Fact]
    public void GivenAnySome_WhenMapOrElse_ThenShouldReturnMappedValueWithoutInvokingFallback()
    {
        Option<string> sut = Any.Some("foo");
        var fallback = Substitute.For<Func<int>>();

        var result = sut.MapOrElse(v => v.Length, fallback);

        Assert.Equal(3, result);
        fallback.DidNotReceive().Invoke();
    }

    [Fact]
    public void GivenAnyNone_WhenMapOrElse_ThenShouldInvokeFallback()
    {
        Option<string> sut = Any.None();
        var fallback = Substitute.For<Func<int>>();
        fallback.Invoke().Returns(42);

        var result = sut.MapOrElse(v => v.Length, fallback);

        Assert.Equal(42, result);
        fallback.Received(1).Invoke();
    }

    [Fact]
    public void GivenAnySome_WhenMapOrDefault_ThenShouldReturnMappedValue()
    {
        Option<string> sut = Any.Some("hi");
        Assert.Equal(2, sut.MapOrDefault(s => s.Length));
    }

    [Fact]
    public void GivenAnyNone_WhenMapOrDefault_ThenShouldReturnTypeDefault()
    {
        Option<string> sut = Any.None();
        Assert.Equal(0, sut.MapOrDefault(s => s.Length));
    }

    [Fact]
    public void GivenAnySome_WhenAnd_ThenShouldReturnOtherOption()
    {
        Option<int> x = Any.Some(2);
        Option<string> y = Any.Some("foo");
        Option<string> none = Any.None();

        Assert.Equal(y, x.And(y));
        Assert.IsNone(x.And(none));
    }

    [Fact]
    public void GivenAnyNone_WhenAnd_ThenShouldReturnNone()
    {
        Option<int> x = Any.None();
        Option<string> y = Any.Some("foo");

        Assert.IsNone(x.And(y));
    }

    [Fact]
    public void GivenAnySome_WhenAndThen_ThenShouldInvokeBoundFunction()
    {
        Option<int> sut = Any.Some(2);
        var fn = Substitute.For<Func<int, Option<string>>>();
        Option<string> expected = Any.Some("4");
        fn.Invoke(2).Returns(expected);

        var result = sut.AndThen(fn);

        Assert.Equal(expected, result);
        fn.Received(1).Invoke(2);
    }

    [Fact]
    public void GivenAnyNone_WhenAndThen_ThenShouldReturnNoneWithoutInvokingFunction()
    {
        Option<int> sut = Any.None();
        var fn = Substitute.For<Func<int, Option<string>>>();

        var result = sut.AndThen(fn);

        Assert.IsNone(result);
        fn.DidNotReceive().Invoke(Arg.Any<int>());
    }

    // [Fact]
    // public void GivenAnySome_WhenOkOr_ThenShouldReturnOk()
    // {
    //     Option<string> sut = Any.Some("foo");
    //     Result<string, int> expected = Any.Ok("foo");
    //
    //     Assert.Equal(expected, sut.OkOr(0));
    // }
    //
    // [Fact]
    // public void GivenAnyNone_WhenOkOr_ThenShouldReturnError()
    // {
    //     Option<string> sut = Any.None();
    //     Result<string, int> expected = Any.Error(0);
    //
    //     Assert.Equal(expected, sut.OkOr(0));
    // }
    //
    // [Fact]
    // public void GivenAnySome_WhenOkOrElse_ThenShouldReturnOkWithoutInvokingErrorFactory()
    // {
    //     Option<string> sut = Any.Some("foo");
    //     var errFactory = Substitute.For<Func<int>>();
    //
    //     var result = sut.OkOrElse(errFactory);
    //
    //     Assert.Equal<Result<string, int>>(Any.Ok("foo"), result);
    //     errFactory.DidNotReceive().Invoke();
    // }
    //
    // [Fact]
    // public void GivenAnyNone_WhenOkOrElse_ThenShouldInvokeErrorFactoryAndReturnError()
    // {
    //     Option<string> sut = Any.None();
    //     var errFactory = Substitute.For<Func<int>>();
    //     errFactory.Invoke().Returns(99);
    //
    //     var result = sut.OkOrElse(errFactory);
    //
    //     Assert.Equal<Result<string, int>>(Any.Error(99), result);
    //     errFactory.Received(1).Invoke();
    // }
    //
    // [Fact]
    // public void GivenAnySome_WhenIter_ThenShouldYieldSingleValue()
    // {
    //     Option<int> sut = Any.Some(4);
    //     Assert.Equal([4], sut.Iter());
    // }
    //
    // [Fact]
    // public void GivenAnyNone_WhenIter_ThenShouldYieldEmpty()
    // {
    //     Option<int> sut = Any.None();
    //     Assert.Empty(sut.Iter());
    // }

    [Fact]
    public void GivenAnyOption_WhenFilter_ThenShouldFilterByPredicate()
    {
        Func<int, bool> isEven = n => n % 2 == 0;

        Option<int> none = Any.None();
        Option<int> odd = Any.Some(3);
        Option<int> even = Any.Some(4);

        Assert.IsNone(none.Filter(isEven));
        Assert.IsNone(odd.Filter(isEven));
        Assert.Equal(4, Assert.IsSome(even.Filter(isEven)).Value);
    }

    [Fact]
    public void GivenAnyOption_WhenOr_ThenShouldReturnFirstSomeOrSecond()
    {
        Option<int> some2 = Any.Some(2);
        Option<int> some100 = Any.Some(100);
        Option<int> none = Any.None();

        Assert.Equal(some2, some2.Or(none));
        Assert.Equal(some100, none.Or(some100));
        Assert.Equal(some2, some2.Or(some100));
        Assert.IsNone(none.Or(none));
    }

    [Fact]
    public void GivenAnySome_WhenOrElse_ThenShouldReturnSelfWithoutInvokingFallback()
    {
        Option<string> sut = Any.Some("barbarians");
        var fallback = Substitute.For<Func<Option<string>>>();

        var result = sut.OrElse(fallback);

        Assert.Equal(sut, result);
        fallback.DidNotReceive().Invoke();
    }

    [Fact]
    public void GivenAnyNone_WhenOrElse_ThenShouldInvokeFallback()
    {
        Option<string> sut = Any.None();
        Option<string> vikings = Any.Some("vikings");
        var fallback = Substitute.For<Func<Option<string>>>();
        fallback.Invoke().Returns(vikings);

        var result = sut.OrElse(fallback);

        Assert.Equal(vikings, result);
        fallback.Received(1).Invoke();
    }

    [Fact]
    public void GivenAnyOptions_WhenXor_ThenShouldReturnSomeOnlyIfExactlyOneIsSome()
    {
        Option<int> some2 = Any.Some(2);
        Option<int> some3 = Any.Some(3);
        Option<int> none = Any.None();

        Assert.Equal(some2, some2.Xor(none));
        Assert.Equal(some2, none.Xor(some2));
        Assert.IsNone(some2.Xor(some3));
        Assert.IsNone(none.Xor(none));
    }

    [Fact]
    public void GivenAnyOptions_WhenZip_ThenShouldReturnTupleOnlyWhenBothAreSome()
    {
        Option<int> x = Any.Some(1);
        Option<string> y = Any.Some("hi");
        Option<byte> z = Any.None();

        Assert.Equal((1, "hi"), Assert.IsSome(x.Zip(y)).Value);
        Assert.IsNone(x.Zip(z));
        Assert.IsNone(z.Zip(y));
    }

    [Fact]
    public void GivenAnyOptions_WhenZipWith_ThenShouldCombineOnlyWhenBothAreSome()
    {
        Option<double> x = Any.Some(17.5);
        Option<double> y = Any.Some(42.7);
        Option<double> none = Any.None();

        Assert.Equal((17.5, 42.7), Assert.IsSome(x.ZipWith(y, (a, b) => (a, b))).Value);
        Assert.IsNone(x.ZipWith(none, (a, b) => (a, b)));
        Assert.IsNone(none.ZipWith(y, (a, b) => (a, b)));
    }

    [Fact]
    public void GivenAnyOptions_WhenReduce_ThenShouldCombineBothOrReturnSingleSome()
    {
        Option<int> s12 = Any.Some(12);
        Option<int> s17 = Any.Some(17);
        Option<int> n = Any.None();

        Func<int, int, int> add = (a, b) => a + b;

        Assert.Equal(29, Assert.IsSome(s12.Reduce(s17, add)).Value);
        Assert.Equal(12, Assert.IsSome(s12.Reduce(n, add)).Value);
        Assert.Equal(17, Assert.IsSome(n.Reduce(s17, add)).Value);
        Assert.IsNone(n.Reduce(n, add));
    }

    [Fact]
    public void GivenNestedOption_WhenFlatten_ThenShouldUnnestOneLevel()
    {
        Option<Option<int>> someSome = Any.Some<Option<int>>(Any.Some(6));
        Option<Option<int>> someNone = Any.Some<Option<int>>(Any.None());
        Option<Option<int>> none = Any.None();

        Assert.Equal(6, Assert.IsSome(someSome.Flatten()).Value);
        Assert.IsNone(someNone.Flatten());
        Assert.IsNone(none.Flatten());
    }

    [Fact]
    public void GivenOptionOfTuple_WhenUnzip_ThenShouldReturnTupleOfOptions()
    {
        Option<(int, string)> someTuple = Any.Some((1, "hi"));
        Option<(byte, int)> noneTuple = Any.None();

        (var a, var b) = someTuple.Unzip();
        Assert.Equal(1, Assert.IsSome(a).Value);
        Assert.Equal("hi", Assert.IsSome(b).Value);

        (var c, var d) = noneTuple.Unzip();
        Assert.IsNone(c);
        Assert.IsNone(d);
    }

    // [Fact]
    // public void GivenOptionOfResult_WhenTranspose_ThenShouldReturnResultOfOption()
    // {
    //     Option<Result<int, string>> someOk = Any.Some<Result<int, string>>(Any.Ok(5));
    //     Option<Result<int, string>> someErr = Any.Some<Result<int, string>>(Any.Error("err"));
    //     Option<Result<int, string>> none = Any.None();
    //
    //     Assert.Equal<Result<Option<int>, string>>(Any.Ok<Option<int>>(Any.Some(5)), someOk.Transpose());
    //     Assert.Equal<Result<Option<int>, string>>(Any.Error("err"), someErr.Transpose());
    //     Assert.Equal<Result<Option<int>, string>>(Any.Ok<Option<int>>(Any.None()), none.Transpose());
    // }
    //
    // [Fact]
    // public void GivenOptionOfEnumerable_WhenIntoFlatIter_ThenShouldYieldElementsOrEmpty()
    // {
    //     Option<IEnumerable<int>> someSeq = Any.Some<IEnumerable<int>>([1, 2]);
    //     Option<IEnumerable<int>> noneSeq = Any.None();
    //
    //     Assert.Equal([1, 2], someSeq.IntoFlatIter());
    //     Assert.Empty(noneSeq.IntoFlatIter());
    // }
}
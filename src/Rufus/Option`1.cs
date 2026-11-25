namespace System;

using static Option;

/// <summary>
///     <see cref="Option{T}" /> represents an optional value: Every <see cref="Option{T}" /> is either
///     <see cref="Some{T}" /> which contains a value, or <see cref="None" />, which does not.
/// </summary>
/// <typeparam name="T">Type of the optional value.</typeparam>
#if NET11_0_OR_GREATER
[Runtime.CompilerServices.Union]
public readonly struct Option<T> : Runtime.CompilerServices.IUnion
    where T : notnull
{
    private readonly T? _value;

    /// <summary>
    ///     Creates a new instance of <see cref="Option{T}" /> containing the given <see cref="Some{T}" /> value.
    /// </summary>
    public Option(in Some<T> some) => (_value, HasValue) = (some.Value, true);

    /// <summary>
    ///     Creates a new instance of <see cref="Option{T}" /> with no value.
    /// </summary>
    public Option(in None _) => _value = default;

    /// <summary>
    ///     Checks whether the result has been initialized with any variant value.
    /// </summary>
    public bool HasValue { get; }

    /// <inheritdoc />
    public object? Value => HasValue ? _value : null;

    /// <summary>
    ///     Checks type-pattern of variant <see cref="Option.Some{T}" /> value.
    /// </summary>
    /// <param name="value">The error value.</param>
    /// <returns><c>true</c> if the result is <see cref="Option.Some{T}" />, <c>false</c> otherwise.</returns>
    public bool TryGetValue(out Some<T> value)
    {
        if (HasValue)
        {
            value = new Some<T>(_value!);

            return true;
        }

        value = default;

        return false;
    }

    /// <summary>
    ///     Checks type-pattern of variant <see cref="Option.None" /> value.
    /// </summary>
    /// <param name="value">The error value.</param>
    /// <returns><c>true</c> if the result is <see cref="Option.None" />, <c>false</c> otherwise.</returns>
    public bool TryGetValue(out None value)
    {
        value = None.Value;

        return !HasValue;
    }
#else
public abstract record Option<T>
    where T : notnull
{
    /// <summary>
    ///     Represents an optional value containing some value.
    /// </summary>
    public sealed record Some(T Value) : Option<T>, Some<T>;

    /// <summary>
    ///     Represents an optional value containing no value.
    /// </summary>
    public sealed record None : Option<T>, Option.None
    {
        internal static readonly None Value = new();
    }

    /// <summary>
    ///     Converts the given <see cref="Values.Some{T}" /> variant that contains some value.
    /// </summary>
    public static implicit operator Option<T>(Option.Values.Some<T> some) => new Some(some.Value);

    /// <summary>
    ///     Converts the given <see cref="Values.None" /> variant that contains no value.
    /// </summary>
    public static implicit operator Option<T>(Option.Values.None _) => None.Value;

#endif

    /// <summary>
    ///     Checks whether current option contains some value.
    /// </summary>
    /// <example>
    ///     <code>
    /// Option&lt;int> x = Option.Some(2);
    /// Assert.True(x.IsSome);
    ///
    /// x = Option.None();
    /// Assert.False(x.IsSome);
    ///     </code>
    /// </example>
    public bool IsSome
#if NET11_0_OR_GREATER
        => HasValue;
#else
        => this is Some;
#endif

    /// <summary>
    ///     Checks whether current option contains no value.
    /// </summary>
    /// <example>
    ///     <code>
    /// Option&lt;int> x = Option.Some(2);
    /// Assert.False(x.IsNone);
    ///
    /// x = Option.None();
    /// Assert.True(x.IsNone);
    ///     </code>
    /// </example>
    public bool IsNone
#if NET11_0_OR_GREATER
        => !HasValue;
#else
        => this is None;
#endif

    /// <summary>
    ///     Returns the contained <see cref="Some{T}" /> value.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;string&gt; x = Option.Some("value");
    ///     Assert.Equal("value", x.Expect("fruits are healthy");
    ///
    ///     x = Option.None();
    ///     Assert.Equal(
    ///         "fruits are healthy",
    ///         Assert.Throws&lt;InvalidOperationException&gt;(() => x.Expect("fruits are healthy")).Message);
    ///     </code>
    /// </example>
    /// <param name="msg">The message of the exception thrown in case of <see cref="None" />.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if current option is not <see cref="Some{T}" /> with the message
    ///     specified on <paramref name="msg" />.
    /// </exception>
    public T Expect(string msg) =>
        this switch
        {
            Some<T>(var value) => value,
            _ => throw new InvalidOperationException(msg),
        };

    /// <summary>
    ///     Calls the specified action with the contained value if <see cref="Some{T}" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;int&gt; x = Option.Some(2).Inspect(v =&gt; Console.WriteLine($"got: {v}"));
    ///     </code>
    /// </example>
    /// <param name="fn">The callback action invoked when <see cref="Some{T}" />.</param>
    /// <returns>Returns the original option.</returns>
    public Option<T> Inspect(Action<T> fn)
    {
#if NET11_0_OR_GREATER
        if (HasValue)
        {
            fn(_value!);
        }
#else
        if (this is Some(var value))
        {
            fn(value);
        }
#endif

        return this;
    }

    /// <summary>
    ///     Checks whether the current option contains a value which matches the predicate.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;int&gt; = Option.Some(2);
    ///     Assert.True(x.IsSomeAnd(x =&gt; x &gt; 1);
    ///
    ///     x = Option.Some(0);
    ///     Assert.False(x.IsSomeAnd(x =&gt; x &gt; 1);
    ///
    ///     x = Option.None();
    ///     Assert.False(x.IsSomeAnd(x =&gt; x &gt; 1);
    ///     </code>
    /// </example>
    /// <param name="predicate">The predicate the value should match.</param>
    /// <returns>
    ///     <c>true</c> if the option is <see cref="Some{T}" /> and the value inside of it matches
    ///     <paramref name="predicate" />.
    /// </returns>
    public bool IsSomeAnd(Func<T, bool> predicate)
#if NET11_0_OR_GREATER
        => HasValue && predicate(_value!);
#else
        => this is Some(var value) && predicate(value);
#endif

    /// <summary>
    ///     Checks whether current option contains no value or value matches specified predicate.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;int&gt; x = Option.Some(2);
    ///     Assert.True(x =&gt; x &gt; 1);
    ///
    ///     x = Option.Some(0);
    ///     Assert.False(x =&gt; x &gt; 1);
    ///
    ///     x = Option.None();
    ///     Assert.True(x =&gt; x &gt; 1);
    ///     </code>
    /// </example>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool IsNoneOr(Func<T, bool> predicate)
#if NET11_0_OR_GREATER
        => !HasValue || predicate(_value!);
#else
        => this is not Some(var value) || predicate(value);
#endif

    /// <summary>
    ///     Maps an <see cref="Option{T}" /> to <see cref="Option{TMap}" /> by applying a function to a contained
    ///     <see cref="Some{T}" /> value, or returns <see cref="None" /> if the option is <see cref="None" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;string&gt; maybeSomeString = Option.Some("Hello, World!");
    ///     Option&lt;int&gt; maybeSomeLen = maybeSomeString.Map(s =&gt; s.Length);
    ///     Assert.Equal(Option.Some(13), maybeSomeLen);
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the contained value.</param>
    /// <typeparam name="TMap">Type of the mapped value.</typeparam>
    /// <returns>
    ///     A new <see cref="Option{TMap}" /> containing the mapped value if <see cref="Some{T}" />,
    ///     otherwise <see cref="None" />.
    /// </returns>
    public Option<TMap> Map<TMap>(Func<T, TMap> map)
        where TMap : notnull =>
#if NET11_0_OR_GREATER
        HasValue ? new Some<TMap>(map(_value!)) : None.Value;
#else
        this is Some(var value) ? new Option<TMap>.Some(map(value)) : Option<TMap>.None.Value;
#endif

    /// <summary>
    ///     Returns the provided <paramref name="default" /> result if <see cref="None" />, or applies
    ///     <paramref name="map" /> to the contained value if <see cref="Some{T}" />.
    /// </summary>
    /// <remarks>
    ///     Arguments passed to <see cref="MapOr{TMap}" /> are eagerly evaluated; if you are passing the result
    ///     of a function call, it is recommended to use <see cref="MapOrElse{TMap}" />, which is lazily evaluated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Option&lt;string&gt; x = Option.Some("foo");
    ///     Assert.Equal(3, x.MapOr(v =&gt; v.Length, 42));
    ///
    ///     x = Option.None();
    ///     Assert.Equal(42, x.MapOr(v =&gt; v.Length, 42));
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the contained value.</param>
    /// <param name="default">The default value returned in case of <see cref="None" />.</param>
    /// <typeparam name="TMap">Type of the mapped value.</typeparam>
    /// <returns>
    ///     The mapped value if <see cref="Some{T}" />, otherwise <paramref name="default" />.
    /// </returns>
    public TMap MapOr<TMap>(Func<T, TMap> map, TMap @default) =>
#if NET11_0_OR_GREATER
        HasValue ? map(_value!) : @default;
#else
            this is Some(var value) ? map(value) : @default;
#endif

    /// <summary>
    ///     Computes a default function result if <see cref="None" />, or applies <paramref name="map" />
    ///     to the contained value if <see cref="Some{T}" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     int k = 21;
    ///     Option&lt;string&gt; x = Option.Some("foo");
    ///     Assert.Equal(3, x.MapOrElse(v =&gt; v.Length, () =&gt; 2 * k));
    ///
    ///     x = Option.None();
    ///     Assert.Equal(42, x.MapOrElse(v =&gt; v.Length, () =&gt; 2 * k));
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the contained value.</param>
    /// <param name="fallback">The function used to compute a fallback value in case of <see cref="None" />.</param>
    /// <typeparam name="TMap">Type of the mapped value.</typeparam>
    /// <returns>
    ///     The mapped value if <see cref="Some{T}" />, otherwise the result of <paramref name="fallback" />.
    /// </returns>
    public TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TMap> fallback) =>
#if NET11_0_OR_GREATER
        HasValue ? map(_value!) : fallback();
#else
            this is Some(var value) ? map(value) : fallback();
#endif

    /// <summary>
    ///     Maps an <see cref="Option{T}" /> to <typeparamref name="TMap" /> by applying <paramref name="map" />
    ///     to the contained value if <see cref="Some{T}" />, otherwise returns the default value for
    ///     <typeparamref name="TMap" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;string&gt; x = Option.Some("hi");
    ///     Option&lt;string&gt; y = Option.None();
    ///     Assert.Equal(2, x.MapOrDefault(s =&gt; s.Length));
    ///     Assert.Equal(0, y.MapOrDefault(s =&gt; s.Length));
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the contained value.</param>
    /// <typeparam name="TMap">Type of the mapped value.</typeparam>
    /// <returns>
    ///     The mapped value if <see cref="Some{T}" />, otherwise <c>default(TMap)</c>.
    /// </returns>
    public TMap? MapOrDefault<TMap>(Func<T, TMap> map) =>
#if NET11_0_OR_GREATER
        HasValue ? map(_value!) : default;
#else
            this is Some(var value) ? map(value) : default;
#endif

    /// <summary>
    ///     Returns the contained <see cref="Some{T}" /> value.
    /// </summary>
    /// <remarks>
    ///     Because this function may throw an exception, its use is generally discouraged.
    ///     Instead, prefer to use pattern matching and handle the <see cref="None" /> case explicitly,
    ///     or call <see cref="UnwrapOr" />, <see cref="UnwrapOrElse" />, or <see cref="UnwrapOrDefault" />.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Option&lt;string&gt; x = Option.Some("air");
    ///     Assert.Equal("air", x.Unwrap());
    ///
    ///     x = Option.None();
    ///     Assert.Throws&lt;InvalidOperationException&gt;(() =&gt; x.Unwrap());
    ///     </code>
    /// </example>
    /// <returns>The contained <see cref="Some{T}" /> value.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the current option is <see cref="None" />.
    /// </exception>
    public T Unwrap()
    {
#if NET11_0_OR_GREATER
        return HasValue
            ? _value!
#else
        return this is Some(var value)
            ? value
#endif
            : throw new InvalidOperationException("Cannot unwrap a None option");
    }

    /// <summary>
    ///     Returns the contained <see cref="Some{T}" /> value or a provided default.
    /// </summary>
    /// <remarks>
    ///     Arguments passed to <see cref="UnwrapOr" /> are eagerly evaluated; if you are passing the result
    ///     of a function call, it is recommended to use <see cref="UnwrapOrElse" />, which is lazily evaluated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Assert.Equal("car", Option.Some("car").UnwrapOr("bike"));
    ///     Assert.Equal("bike", Option&lt;string&gt;.None.Value.UnwrapOr("bike"));
    ///     </code>
    /// </example>
    /// <param name="default">The default value returned in case of <see cref="None" />.</param>
    /// <returns>The contained value if <see cref="Some{T}" />, otherwise <paramref name="default" />.</returns>
    public T UnwrapOr(T @default) =>
#if NET11_0_OR_GREATER
        HasValue ? _value! : @default;
#else
        this is Some(var value) ? value : @default;
#endif

    /// <summary>
    ///     Returns the contained <see cref="Some{T}" /> value or computes it from a closure.
    /// </summary>
    /// <example>
    ///     <code>
    ///     int k = 10;
    ///     Assert.Equal(4, Option.Some(4).UnwrapOrElse(() =&gt; 2 * k));
    ///     Assert.Equal(20, Option&lt;int&gt;.None.Value.UnwrapOrElse(() =&gt; 2 * k));
    ///     </code>
    /// </example>
    /// <param name="op">The function that computes a fallback value in case of <see cref="None" />.</param>
    /// <returns>The contained value if <see cref="Some{T}" />, otherwise the result of <paramref name="op" />.</returns>
    public T UnwrapOrElse(Func<T> op) =>
#if NET11_0_OR_GREATER
        HasValue ? _value! : op();
#else
        this is Some(var value) ? value : op();
#endif

    /// <summary>
    ///     Returns the contained <see cref="Some{T}" /> value or the default value of <typeparamref name="T" />.
    /// </summary>
    /// <remarks>
    ///     Take in account that, although <typeparamref name="T" /> is defined as <c>notnull</c>, this method can return
    ///     <c>null</c> when <typeparamref name="T" /> is a reference type.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Option&lt;int&gt; x = Option.None();
    ///     Option&lt;int&gt; y = Option.Some(12);
    ///     Assert.Equal(0, x.UnwrapOrDefault());
    ///     Assert.Equal(12, y.UnwrapOrDefault());
    ///     </code>
    /// </example>
    /// <returns>
    ///     The contained value if <see cref="Some{T}" />, otherwise <c>default(T)</c>.
    /// </returns>
    public T? UnwrapOrDefault() =>
#if NET11_0_OR_GREATER
        HasValue ? _value! : default;
#else
        this is Some(var value) ? value : default;
#endif

    //
    //     /// <summary>
    //     ///     Transforms the <see cref="Option{T}" /> into a <see cref="Result{T, TError}" />, mapping
    //     ///     <see cref="Some{T}" /> to <see cref="Result.Ok{T}" /> and <see cref="None" /> to
    //     ///     <see cref="Result.Error{TError}" /> with <paramref name="error" />.
    //     /// </summary>
    //     /// <remarks>
    //     ///     Arguments passed to <see cref="OkOr{TError}" /> are eagerly evaluated; if you are passing the result
    //     ///     of a function call, it is recommended to use <see cref="OkOrElse{TError}" />, which is lazily evaluated.
    //     /// </remarks>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;string&gt; x = Option.Some("foo");
    //     ///     Assert.Equal(Result.Ok("foo"), x.OkOr(0));
    //     ///
    //     ///     x = Option.None();
    //     ///     Assert.Equal(Result.Error(0), x.OkOr(0));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="error">The error value returned in case of <see cref="None" />.</param>
    //     /// <typeparam name="TError">Type of the error value.</typeparam>
    //     /// <returns>
    //     ///     <see cref="Result{T, TError}" /> containing the value if <see cref="Some{T}" />,
    //     ///     otherwise <paramref name="error" />.
    //     /// </returns>
    //     public Result<T, TError> OkOr<TError>(TError error)
    //         where TError : notnull =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? new Result.Ok<T>(_value!) : new Result.Error<TError>(error);
    // #else
    //         this is Some<T>(var value) ? new Result<T, TError>.Ok(value) : new Result<T, TError>.Error(error);
    // #endif
    //
    //     /// <summary>
    //     ///     Transforms the <see cref="Option{T}" /> into a <see cref="Result{T, TError}" />, mapping
    //     ///     <see cref="Some{T}" /> to <see cref="Result.Ok{T}" /> and <see cref="None" /> to
    //     ///     <see cref="Result.Error{TError}" /> computed from <paramref name="err" />.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;string&gt; x = Option.Some("foo");
    //     ///     Assert.Equal(Result.Ok("foo"), x.OkOrElse(() =&gt; 0));
    //     ///
    //     ///     x = Option.None();
    //     ///     Assert.Equal(Result.Error(0), x.OkOrElse(() =&gt; 0));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="err">The function that computes the error value in case of <see cref="None" />.</param>
    //     /// <typeparam name="TError">Type of the error value.</typeparam>
    //     /// <returns>
    //     ///     <see cref="Result{T, TError}" /> containing the value if <see cref="Some{T}" />,
    //     ///     otherwise the error produced by <paramref name="err" />.
    //     /// </returns>
    //     public Result<T, TError> OkOrElse<TError>(Func<TError> err)
    //         where TError : notnull =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? new Result.Ok<T>(_value!) : new Result.Error<TError>(err());
    // #else
    //         this is Some<T>(var value) ? new Result<T, TError>.Ok(value) : new Result<T, TError>.Error(err());
    // #endif
    //
    //     /// <summary>
    //     ///     Returns an enumerator over the possibly contained value.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;int&gt; x = Option.Some(4);
    //     ///     Assert.Equal([4], x.Iter());
    //     ///
    //     ///     x = Option.None();
    //     ///     Assert.Empty(x.Iter());
    //     ///     </code>
    //     /// </example>
    //     /// <returns>
    //     ///     An <see cref="IEnumerable{T}" /> yielding the contained value if <see cref="Some{T}" />,
    //     ///     or an empty sequence if <see cref="None" />.
    //     /// </returns>
    //     public IEnumerable<T> Iter()
    //     {
    // #if NET11_0_OR_GREATER
    //         if (HasValue)
    //         {
    //             yield return _value!;
    //         }
    // #else
    //         if (this is Some<T>(var value))
    //         {
    //             yield return value;
    //         }
    // #endif
    //     }
    //
    //     /// <summary>
    //     ///     Returns <see cref="None" /> if the option is <see cref="None" />, otherwise returns <paramref name="option" />.
    //     /// </summary>
    //     /// <remarks>
    //     ///     Arguments passed to <see cref="And{TMap}" /> are eagerly evaluated; if you are passing the result of a
    //     ///     function call, it is recommended to use <see cref="AndThen{TMap}" />, which is lazily evaluated.
    //     /// </remarks>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;int&gt; x = Option.Some(2);
    //     ///     Option&lt;string&gt; y = Option.None();
    //     ///     Assert.Equal(Option.None(), x.And(y));
    //     ///
    //     ///     y = Option.Some("foo");
    //     ///     Assert.Equal(Option.Some("foo"), x.And(y));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="option">The option to return if the current option is <see cref="Some{T}" />.</param>
    //     /// <typeparam name="TMap">Type of the returned option's value.</typeparam>
    //     /// <returns>
    //     ///     <paramref name="option" /> if <see cref="Some{T}" />, otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<TMap> And<TMap>(Option<TMap> option)
    //         where TMap : notnull =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? option : Option.None.Value;
    // #else
    //         this is Some<T> ? option : Option<TMap>.None.Value;
    // #endif
    //
    //     /// <summary>
    //     ///     Returns <see cref="None" /> if the option is <see cref="None" />, otherwise calls <paramref name="fn" />
    //     ///     with the wrapped value and returns the result.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     static Option&lt;string&gt; SqThenToString(int x) =&gt;
    //     ///         x &lt;= 1000 ? Option.Some((x * x).ToString()) : Option.None();
    //     ///
    //     ///     Assert.Equal(Option.Some("4"), Option.Some(2).AndThen(SqThenToString));
    //     ///     Assert.Equal(Option.None(), Option.Some(1_000_000).AndThen(SqThenToString));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="fn">The function to invoke with the contained value.</param>
    //     /// <typeparam name="TMap">Type of the returned option's value.</typeparam>
    //     /// <returns>
    //     ///     The result of <paramref name="fn" /> if <see cref="Some{T}" />, otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<TMap> AndThen<TMap>(Func<T, Option<TMap>> fn)
    //         where TMap : notnull =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? fn(_value!) : Option.None.Value;
    // #else
    //         this is Some<T>(var value) ? fn(value) : Option<TMap>.None.Value;
    // #endif
    //
    //     /// <summary>
    //     ///     Returns <see cref="None" /> if the option is <see cref="None" />, otherwise calls
    //     ///     <paramref name="predicate" /> with the wrapped value and returns <see cref="Some{T}" /> if
    //     ///     <paramref name="predicate" /> returns <c>true</c>, or <see cref="None" /> if it returns <c>false</c>.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     static bool IsEven(int n) =&gt; n % 2 == 0;
    //     ///
    //     ///     Assert.Equal(Option.None(), Option&lt;int&gt;.None.Value.Filter(IsEven));
    //     ///     Assert.Equal(Option.None(), Option.Some(3).Filter(IsEven));
    //     ///     Assert.Equal(Option.Some(4), Option.Some(4).Filter(IsEven));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="predicate">The predicate used to test the contained value.</param>
    //     /// <returns>
    //     ///     The current option if <see cref="Some{T}" /> and <paramref name="predicate" /> returns <c>true</c>,
    //     ///     otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<T> Filter(Func<T, bool> predicate) =>
    // #if NET11_0_OR_GREATER
    //         HasValue && predicate(_value!) ? this : Option.None.Value;
    // #else
    //         this is Some<T>(var value) && predicate(value) ? this : None.Value;
    // #endif
    //
    //     /// <summary>
    //     ///     Returns the option if it contains a value, otherwise returns <paramref name="option" />.
    //     /// </summary>
    //     /// <remarks>
    //     ///     Arguments passed to <see cref="Or" /> are eagerly evaluated; if you are passing the result of a
    //     ///     function call, it is recommended to use <see cref="OrElse" />, which is lazily evaluated.
    //     /// </remarks>
    //     /// <example>
    //     ///     <code>
    //     ///     Assert.Equal(Option.Some(2), Option.Some(2).Or(Option.None()));
    //     ///     Assert.Equal(Option.Some(100), Option&lt;int&gt;.None.Value.Or(Option.Some(100)));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="option">The fallback option returned when the current option is <see cref="None" />.</param>
    //     /// <returns>The current option if <see cref="Some{T}" />, otherwise <paramref name="option" />.</returns>
    //     public Option<T> Or(Option<T> option) =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? this : option;
    // #else
    //         this is Some<T> ? this : option;
    // #endif
    //
    //     /// <summary>
    //     ///     Returns the option if it contains a value, otherwise calls <paramref name="fn" /> and returns the result.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;string&gt; Vikings() =&gt; Option.Some("vikings");
    //     ///
    //     ///     Assert.Equal(Option.Some("barbarians"), Option.Some("barbarians").OrElse(Vikings));
    //     ///     Assert.Equal(Option.Some("vikings"), Option&lt;string&gt;.None.Value.OrElse(Vikings));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="fn">The function that computes a fallback option in case of <see cref="None" />.</param>
    //     /// <returns>The current option if <see cref="Some{T}" />, otherwise the result of <paramref name="fn" />.</returns>
    //     public Option<T> OrElse(Func<Option<T>> fn) =>
    // #if NET11_0_OR_GREATER
    //         HasValue ? this : fn();
    // #else
    //         this is Some<T> ? this : fn();
    // #endif
    //
    //     /// <summary>
    //     ///     Returns <see cref="Some{T}" /> if exactly one of <c>this</c> and <paramref name="option" /> is
    //     ///     <see cref="Some{T}" />, otherwise returns <see cref="None" />.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Assert.Equal(Option.Some(2), Option.Some(2).Xor(Option.None()));
    //     ///     Assert.Equal(Option.Some(2), Option&lt;int&gt;.None.Value.Xor(Option.Some(2)));
    //     ///     Assert.Equal(Option.None(), Option.Some(2).Xor(Option.Some(2)));
    //     ///     Assert.Equal(Option.None(), Option&lt;int&gt;.None.Value.Xor(Option.None()));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="option">The other option to combine with.</param>
    //     /// <returns>
    //     ///     The single <see cref="Some{T}" /> option if exactly one is <see cref="Some{T}" />,
    //     ///     otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<T> Xor(Option<T> option) =>
    //         (IsSome, option.IsSome) switch
    //         {
    //             (true, false) => this,
    //             (false, true) => option,
    //             _ =>
    // #if NET11_0_OR_GREATER
    //                 Option.None.Value,
    // #else
    //                 None.Value,
    // #endif
    //         };
    //
    //     /// <summary>
    //     ///     Makes a tuple of the value in <c>this</c> and the value in <paramref name="other" />.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;int&gt; x = Option.Some(1);
    //     ///     Option&lt;string&gt; y = Option.Some("hi");
    //     ///     Option&lt;byte&gt; z = Option.None();
    //     ///
    //     ///     Assert.Equal(Option.Some((1, "hi")), x.Zip(y));
    //     ///     Assert.Equal(Option.None(), x.Zip(z));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="other">The other option to zip with.</param>
    //     /// <typeparam name="TOther">Type of the other option's value.</typeparam>
    //     /// <returns>
    //     ///     <see cref="Some{T}" /> containing <c>(s, o)</c> if both options are <see cref="Some{T}" />,
    //     ///     otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<(T, TOther)> Zip<TOther>(Option<TOther> other)
    //         where TOther : notnull =>
    //         (this, other) switch
    //         {
    // #if NET11_0_OR_GREATER
    //             ({ HasValue: true, _value: var s }, { HasValue: true, _value: var o }) => new Some<(T, TOther)>((s!, o!)),
    //             _ => Option.None.Value,
    // #else
    //             (Some<T>(var s), Some<TOther>(var o)) => new Option<(T, TOther)>.Some((s, o)),
    //             _ => Option<(T, TOther)>.None.Value,
    // #endif
    //         };
    //
    //     /// <summary>
    //     ///     Combines the value in <c>this</c> with the value in <paramref name="other" />, using <paramref name="fn" />.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;double&gt; x = Option.Some(17.5);
    //     ///     Option&lt;double&gt; y = Option.Some(42.7);
    //     ///     Assert.Equal(Option.Some((17.5, 42.7)), x.ZipWith(y, (a, b) =&gt; (a, b)));
    //     ///     Assert.Equal(Option.None(), x.ZipWith(Option&lt;double&gt;.None.Value, (a, b) =&gt; (a, b)));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="other">The other option to combine with.</param>
    //     /// <param name="fn">The function used to combine both values.</param>
    //     /// <typeparam name="TOther">Type of the other option's value.</typeparam>
    //     /// <typeparam name="TResult">Type of the combined value.</typeparam>
    //     /// <returns>
    //     ///     <see cref="Some{T}" /> containing <c>fn(s, o)</c> if both options are <see cref="Some{T}" />,
    //     ///     otherwise <see cref="None" />.
    //     /// </returns>
    //     public Option<TResult> ZipWith<TOther, TResult>(Option<TOther> other, Func<T, TOther, TResult> fn)
    //         where TOther : notnull
    //         where TResult : notnull =>
    //         (this, other) switch
    //         {
    // #if NET11_0_OR_GREATER
    //             ({ HasValue: true, _value: var s }, { HasValue: true, _value: var o }) => new Some<TResult>(fn(s!, o!)),
    //             _ => Option.None.Value,
    // #else
    //             (Some<T>(var s), Some<TOther>(var o)) => new Option<TResult>.Some(fn(s, o)),
    //             _ => Option<TResult>.None.Value,
    // #endif
    //         };
    //
    //     /// <summary>
    //     ///     Reduces two options into one, using <paramref name="fn" /> if both are <see cref="Some{T}" />.
    //     ///     If only one of <c>this</c> and <paramref name="other" /> is <see cref="Some{T}" />, that one is returned.
    //     ///     If both are <see cref="None" />, <see cref="None" /> is returned.
    //     /// </summary>
    //     /// <example>
    //     ///     <code>
    //     ///     Option&lt;int&gt; s12 = Option.Some(12);
    //     ///     Option&lt;int&gt; s17 = Option.Some(17);
    //     ///     Option&lt;int&gt; n = Option.None();
    //     ///     Func&lt;int, int, int&gt; f = (a, b) =&gt; a + b;
    //     ///
    //     ///     Assert.Equal(Option.Some(29), s12.Reduce(s17, f));
    //     ///     Assert.Equal(Option.Some(12), s12.Reduce(n, f));
    //     ///     Assert.Equal(Option.Some(17), n.Reduce(s17, f));
    //     ///     Assert.Equal(Option.None(), n.Reduce(n, f));
    //     ///     </code>
    //     /// </example>
    //     /// <param name="other">The other option to reduce with.</param>
    //     /// <param name="fn">The function used to combine both values when both are <see cref="Some{T}" />.</param>
    //     /// <returns>The reduced option.</returns>
    //     public Option<T> Reduce(Option<T> other, Func<T, T, T> fn) =>
    //         (this, other) switch
    //         {
    // #if NET11_0_OR_GREATER
    //             ({ HasValue: true, _value: var s }, { HasValue: true, _value: var o }) => new Some<T>(fn(s!, o!)),
    //             ({ HasValue: true }, _) => this,
    //             (_, { HasValue: true }) => other,
    //             _ => Option.None.Value,
    // #else
    //             (Some<T>(var s), Some<T>(var o)) => new Some(fn(s, o)),
    //             (Some<T>, _) => this,
    //             (_, Some<T>) => other,
    //             _ => None.Value,
    // #endif
    //         };
}
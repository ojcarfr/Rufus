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
        => this is Some<T>;
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
        => this is Some<T>(var value) && predicate(value);
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
        => this is not Some<T>(var value) || predicate(value);
#endif
}
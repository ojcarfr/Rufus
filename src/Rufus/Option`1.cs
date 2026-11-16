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
}
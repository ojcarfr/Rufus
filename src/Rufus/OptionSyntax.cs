namespace System;

/// <summary>
///     Defines initialization and common syntax for <see cref="Option" /> values.
/// </summary>
public static class OptionSyntax
{
#if NET10_0_OR_GREATER
    extension(Option)
    {
        /// <summary>
        ///     Returns an <see cref="Option.Some{T}" /> value containing the unit value.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Option.Some<_> Some()
#else
        public static Option.Values.Some<_> Some()
#endif
            => new();

        /// <summary>
        ///     Returns an <see cref="Option.Some{T}" /> value containing the given <paramref name="value" />.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Option.Some<T> Some<T>(T value)
#else
        public static Option.Values.Some<T> Some<T>(T value)
#endif
            where T : notnull => new(value);

        /// <summary>
        ///     Returns an <see cref="Option.None" /> value indicating an optional value containing no value.
        /// </summary>
#if NET11_0_OR_GREATER
    public static Option.None None()
#else
        public static Option.Values.None None()
#endif
            => new();
    }
#endif
}
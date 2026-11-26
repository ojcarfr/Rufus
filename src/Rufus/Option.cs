namespace System;

/// <summary>
///     Contains the discriminated values of an optional value.
/// </summary>
public static class Option
{
    /// <summary>
    ///     Value representing the optional value contains a value.
    /// </summary>
    /// <typeparam name="T">Type of the optional contained value.</typeparam>
#if NET11_0_OR_GREATER
    public readonly record struct Some<T>(T Value)
        where T : notnull;
#else
#pragma warning disable IDE1006 // Naming Styles
    // ReSharper disable once InconsistentNaming
    public interface Some<T>
    {
        /// <summary>
        ///     Gets the contained value.
        /// </summary>
        T Value { get; }

        /// <summary>
        ///     Deconstructs the contained value into its underlying value.
        /// </summary>
        void Deconstruct(out T value);
    }
#pragma warning restore IDE1006 // Naming Styles
#endif

    /// <summary>
    ///     Value representing the optional value contains no value.
    /// </summary>
#if NET11_0_OR_GREATER
    public readonly record struct None
    {
        /// <summary>
        ///     Gets the default value of none that is equal to any other <see cref="None" /> value.
        /// </summary>
        public static readonly None Value = new();

        /// <inheritdoc />
        public bool Equals(None other) => true;

        /// <inheritdoc />
        public override int GetHashCode() => 0;

        /// <inheritdoc />
        public override string ToString() => string.Empty;
    }
#else
#pragma warning disable IDE1006 // Naming Styles
    // ReSharper disable once InconsistentNaming
    public interface None;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    ///     Value implementations of <see cref="Option" /> discriminated values to implicit casting of
    ///     <see cref="Option{T}" />.
    /// </summary>
    public static class Values
    {
        /// <summary>
        ///     Optional value containing some value.
        /// </summary>
        public readonly record struct Some<T>(T Value) : Option.Some<T>;

        /// <summary>
        ///     Optional value containing no value.
        /// </summary>
        public readonly record struct None : Option.None
        {
            /// <summary>
            ///     Gets the default value of none that is equal to any other <see cref="None" /> value.
            /// </summary>
            public static readonly None Value = new();
        }
    }
#endif
}
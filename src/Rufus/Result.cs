namespace System;

using System.Runtime.CompilerServices;

/// <summary>
///     Defines the variant types of <see cref="Result{T, TError}" /> discriminator union.
/// </summary>
/// <remarks>
///     This interface allows casing <see cref="Result{T, TError}" /> values in a less verbose way.
/// </remarks>
public static class Result
{
    internal static SwitchExpressionException Uninitialized()
        => new();

    /// <summary>
    ///     Contains the success value.
    /// </summary>
    /// <typeparam name="T">Type of the success value.</typeparam>
#if NET11_0_OR_GREATER
    public readonly record struct Ok<T>(T Value)
        where T : notnull;
#else
#pragma warning disable IDE1006 // Naming Styles
    // ReSharper disable InconsistentNaming
    public interface Ok<T>
        where T : notnull
    {
        /// <summary>
        ///     Gets the success value.
        /// </summary>
        T Value { get; }

        /// <summary>
        ///     Deconstructs the success result into its underlying value.
        /// </summary>
        void Deconstruct(out T Value);
    }
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
#endif

    /// <summary>
    ///     Contains the error value.
    /// </summary>
    /// <typeparam name="T">Type of the error value.</typeparam>
#if NET11_0_OR_GREATER
    public readonly record struct Error<T>(T Value)
        where T : notnull;
#else
#pragma warning disable IDE1006 // Naming Styles
    // ReSharper disable InconsistentNaming
    public interface Error<T>
        where T : notnull
    {
        /// <summary>
        ///     Gets the error value.
        /// </summary>
        T Value { get; }

        /// <summary>
        ///     Deconstructs the error result into its underlying value.
        /// </summary>
        void Deconstruct(out T Value);
    }
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
#endif
#if !NET11_0_OR_GREATER
    /// <summary>
    ///     Returns a success results of a void operations, containing an unit value.
    /// </summary>
#if NET10_0_OR_GREATER
    [Obsolete("This method will be removed in dotnet 11. Use Result.Ok() instead")]
#endif
    public static Values.Ok<_> Success() => new();

    /// <summary>
    ///     Returns a success value containing the specified <paramref name="value" />.
    ///     This value type should be implicitly converted to a <see cref="Result{T, TError}" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     public static Result&lt;int, string> Divide(int numerator, int denominator) =>
    ///         denominator switch
    ///         {
    ///             0 => Result.Error("Denominator cannot be zero."),
    ///             _ => Result.Ok(numerator / denominator),
    ///         };
    ///     </code>
    /// </example>
#if NET10_0_OR_GREATER
    [Obsolete("This method will be removed in dotnet 11. Use Result.Ok(value) instead")]
#endif
    public static Values.Ok<T> Success<T>(T value)
        where T : notnull => new(value);

    /// <summary>
    ///     Returns a failure value containing the specified <paramref name="value" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     public static Result&lt;int, string> Divide(int numerator, int denominator) =>
    ///         denominator switch
    ///         {
    ///             0 => Result.Error("Denominator cannot be zero."),
    ///             _ => Result.Ok(numerator / denominator),
    ///         };
    ///     </code>
    /// </example>
#if NET10_0_OR_GREATER
    [Obsolete("This method will be removed in dotnet 11. Use Result.Error(value) instead")]
#endif
    public static Values.Error<T> Failure<T>(T value)
        where T : notnull => new(value);

    /// <summary>
    ///     Defines default value type implementations of <see cref="Result{T, TError}" /> variants.
    /// </summary>
    public static class Values
    {
        /// <summary>
        ///     Value type to pass a success value that can be implicitly converted to a proper
        ///     <see cref="Result{T, TError}" /> by avoiding error generic type definition.
        /// </summary>
        public readonly record struct Ok<T>(T Value) : Result.Ok<T>
            where T : notnull;

        /// <summary>
        ///     Value type to pass an error value that can be implicitly converted to a proper
        ///     <see cref="Result{T, TError}" /> by avoiding success generic type definition.
        /// </summary>
        public readonly record struct Error<TError>(TError Value) : Result.Error<TError>
            where TError : notnull;
    }
#endif
}
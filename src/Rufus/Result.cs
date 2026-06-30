namespace System;

#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming
/// <summary>
///     Defines the variant types of <see cref="Result{T, TError}" /> discriminator union.
/// </summary>
/// <remarks>
///     This interface allows casing <see cref="Result{T, TError}" /> values in a less verbose way.
/// </remarks>
public static class Result
{
    /// <summary>
    ///     Contains the success value.
    /// </summary>
    /// <typeparam name="T">Type of the success value.</typeparam>
    public interface Ok<out T>
        where T : notnull
    {
        /// <summary>
        ///     Gets the success value.
        /// </summary>
        T Value { get; }
    }

    /// <summary>
    ///     Contains the error value.
    /// </summary>
    /// <typeparam name="TError">Type of the error value.</typeparam>
    public interface Error<out TError>
        where TError : notnull
    {
        /// <summary>
        ///     Gets the error value.
        /// </summary>
        TError Value { get; }
    }
}
// ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
namespace Rufus;

#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming
/// <summary>
/// Defines a type that represents either success () or failure ().
/// </summary>
/// <remarks>
/// This interface allows casing <see cref="Result{T, TError}"/> values in a less verbose way.
/// </remarks>
public interface Result
{
    /// <summary>
    /// Contains the success value.
    /// </summary>
    /// <typeparam name="T">Type of the success value.</typeparam>
    interface Ok<out T> : Result
    {
        /// <summary>
        /// Gets the success value.
        /// </summary>
        T Value { get; }
    }

    /// <summary>
    /// Contains the error value.
    /// </summary>
    /// <typeparam name="TError">Type of the error value.</typeparam>
    interface Error<out TError> : Result
    {
        /// <summary>
        /// Gets the error value.
        /// </summary>
        TError Value { get; }
    }
}
// ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles

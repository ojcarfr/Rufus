namespace System;

#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming
/// <summary>
///     Contains the error value.
/// </summary>
/// <typeparam name="TError">Type of the error value.</typeparam>
public interface Error<TError>
    where TError : notnull
{
    /// <summary>
    ///     Gets the error value.
    /// </summary>
    TError Value { get; }

    /// <summary>
    ///     Deconstructs the error result into its underlying value.
    /// </summary>
    void Deconstruct(out TError Value);
}
// ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
namespace System;

#pragma warning disable IDE1006 // Naming Styles
// ReSharper disable InconsistentNaming
/// <summary>
///     Contains the success value.
/// </summary>
/// <typeparam name="T">Type of the success value.</typeparam>
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
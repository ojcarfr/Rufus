namespace System;

using System.Diagnostics.CodeAnalysis;

/// <summary>
///     Unit value that returns same input to any pure function.
///     It uses '_' as name in order to use same disregard that variables when declaring generic types.
/// </summary>
/// <remarks>
///     This type is a value type that always is equals to any unit value else.
/// </remarks>
#pragma warning disable CA2231
public readonly struct _ : IEquatable<_>
#pragma warning restore CA2231
{
    /// <inheritdoc />
    public bool Equals(_ other) => true;

    /// <inheritdoc />
    // ReSharper disable once RedundantVerbatimPrefix
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is @_;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public override string ToString()
    {
        const string VALUE = "()";

        return VALUE;
    }
}
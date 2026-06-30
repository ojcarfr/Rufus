namespace System;

using System.Runtime.CompilerServices;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static partial class ResultSyntax
{
    /// <summary>
    ///     Unnest any <see cref="Result{T,TError}" /> that returns any result else in case of <see cref="Ok" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;Result&lt;string, int&gt;, int&gt; x = Result.Ok(Result.Ok("hello"));
    ///     Assert.Equal(Result.Ok("hello"), x.Flatten());
    ///
    ///     x = Result.Ok(Result.Error(6));
    ///     Assert.Equal(Result.Error(6), x.Flatten());
    ///
    ///     x = Result.Error(6);
    ///     Assert.Equal(Result.Error(6), x.Flatten());
    ///     </code>
    /// </example>
    public static Result<T, TError> Flatten<T, TError>(this Result<Result<T, TError>, TError> result)
        where TError : notnull
        where T : notnull
        => result switch
        {
            Result<Result<T, TError>, TError>.Ok(var ok) => ok,
            Result<Result<T, TError>, TError>.Error(var error) => new Result<T, TError>.Error(error),
            _ => throw new SwitchExpressionException(result),
        };

    /// <summary>
    ///     Defines operations to instantiate results in a less verbose fashion relaying on value types that implicitly can be
    ///     converted into a generic result union.
    /// </summary>
    extension(Result)
    {
        /// <summary>
        ///     Returns a success results of a void operations, containing an unit value.
        /// </summary>
        public static Result.Values.Ok<_> Ok() => new();

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
        public static Result.Values.Ok<T> Ok<T>(T value)
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
        public static Result.Values.Error<TError> Error<TError>(TError value)
            where TError : notnull => new(value);
    }
}
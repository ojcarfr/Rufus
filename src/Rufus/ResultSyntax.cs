namespace Rufus;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static class ResultSyntax
{
    /// <summary>
    ///     Defines operations to instantiate results in a less verbose fashion relaying on value types that implicitly can be
    ///     converted into a generic result union.
    /// </summary>
    extension(Result)
    {
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
        public static OkValue<T> Ok<T>(T value) => new(value);

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
        public static ErrorValue<TError> Error<TError>(TError value)
            where TError : notnull => new(value);
    }

    extension<T>(Result.Ok<T> ok)
    {
        /// <summary>
        ///     Binds <paramref name="fn" /> function to be executed if the result is <see cref="Ok" />.
        /// </summary>
        /// <example>
        ///     <code>
        /// static Result&lt;string, string> SqThenToString(int value)
        /// {
        ///     checked
        ///     {
        ///         try
        ///         {
        ///             return Result.Ok((value * value).ToString());
        ///         }
        ///         catch(OverflowException)
        ///         {
        ///             return Result.Error("overflowed");
        ///         }
        ///     }
        /// }
        ///
        /// Assert.Equal(Result.Ok(4.ToString()), Result.Ok(2).AndThen(SqThenToString));
        /// Assert.Equal(Result.Error("overflowed"), Result.Ok(1_000_000).AndThen(SqThenToString));
        /// Assert.Equal(Result.Error("not a number"), Result.Error("not a number").AndThen((int x) => SqThenToString(x)));
        ///     </code>
        /// </example>
        /// <param name="fn">The bound function to the current result.</param>
        /// <typeparam name="TMap">The type of result returned by bound function.</typeparam>
        /// <typeparam name="TError">The type of the error result.</typeparam>
        /// <returns>The result of the bound function if <see cref="Ok" />, same error in case of <see cref="Error" />.</returns>
        public Result<TMap, TError> AndThen<TMap, TError>(Func<T, Result<TMap, TError>> fn)
            where TError : notnull => fn(ok.Value);

        /// <summary>
        ///     Deconstructs the success result into its underlying value.
        /// </summary>
        public void Deconstruct(out T value) => value = ok.Value;
    }

    extension<TError>(Result.Error<TError> error)
        where TError : notnull
    {
        /// <summary>
        ///     Binds a function to be executed if the result is <see cref="Ok" />.
        /// </summary>
        /// <example>
        ///     <code>
        /// static Result&lt;string, string> SqThenToString(int value)
        /// {
        ///     checked
        ///     {
        ///         try
        ///         {
        ///             return Result.Ok((value * value).ToString());
        ///         }
        ///         catch(OverflowException)
        ///         {
        ///             return Result.Error("overflowed");
        ///         }
        ///     }
        /// }
        ///
        /// Assert.Equal(Result.Ok(4.ToString()), Result.Ok(2).AndThen(SqThenToString));
        /// Assert.Equal(Result.Error("overflowed"), Result.Ok(1_000_000).AndThen(SqThenToString));
        /// Assert.Equal(Result.Error("not a number"), Result.Error("not a number").AndThen((int x) => SqThenToString(x)));
        ///     </code>
        /// </example>
        /// <param name="_">The bound function to the current result.</param>
        /// <returns>The result of the bound function if <see cref="Ok" />, same error in case of <see cref="Error" />.</returns>
        public Result<TOutput, TError> AndThen<TInput, TOutput>(Func<TInput, Result<TOutput, TError>> _)
            => Result.Error(error.Value);

        /// <summary>
        ///     Deconstructs the error result into its underlying value.
        /// </summary>
        public void Deconstruct(out TError value) => value = error.Value;
    }

    /// <summary>
    ///     Value type to pass a success value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding error generic type definition.
    /// </summary>
    public readonly record struct OkValue<T>(T Value) : Result.Ok<T>;

    /// <summary>
    ///     Value type to pass an error value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding success generic type definition.
    /// </summary>
    public readonly record struct ErrorValue<TError>(TError Value) : Result.Error<TError>
        where TError : notnull;
}
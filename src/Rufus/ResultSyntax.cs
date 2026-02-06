namespace Rufus;

using System.Runtime.CompilerServices;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static class ResultSyntax
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
        => result switch
        {
            Result<Result<T, TError>, TError>.Ok ok => ok.Value,
            Result<Result<T, TError>, TError>.Error error => new Result<T, TError>.Error(error.Value),
            _ => throw new SwitchExpressionException(result),
        };

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
        ///     Deconstructs the success result into its underlying value.
        /// </summary>
        public void Deconstruct(out T value) => value = ok.Value;
    }

    extension<TError>(Result.Error<TError> error)
        where TError : notnull
    {
        /// <summary>
        ///     Deconstructs the error result into its underlying value.
        /// </summary>
        public void Deconstruct(out TError value) => value = error.Value;
    }

    /// <summary>
    ///     Value type to pass a success value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding error generic type definition.
    /// </summary>
    public readonly record struct OkValue<T>(T Value) : Result.Ok<T>
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
            where TError : notnull => fn(Value);

        /// <summary>
        ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error" />.
        ///     This function can be used for control flow based on result values.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Result&lt;int, int&gt; Sq(int x) => Result.Ok(x * x);
        ///     Result&lt;int, int&gt; Err(int x) => Result.Error(x);
        ///
        ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Sq).OrElse(Sq));
        ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Err).OrElse(Sq));
        ///     Assert.Equal(Result.Ok(9), Result.Error(3).OrElse(Sq).OrElse(Err));
        ///     Assert.Equal(Result.Error(3), Result.Error(3).OrElse(Err).OrElse(Sq));
        ///     </code>
        /// </example>
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMap">Type of the mapped error by the bound function.</typeparam>
        /// <typeparam name="TError">Type of the source result error.</typeparam>
        /// <returns>The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok" /> value.</returns>
        public Result<T, TMap> OrElse<TError, TMap>(Func<TError, Result<T, TMap>> op)
            where TMap : notnull
            where TError : notnull => Result.Ok(Value);
    }

    /// <summary>
    ///     Value type to pass an error value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding success generic type definition.
    /// </summary>
    public readonly record struct ErrorValue<TError>(TError Value) : Result.Error<TError>
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
            => Result.Error(Value);

        /// <summary>
        ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error" />.
        ///     This function can be used for control flow based on result values.
        /// </summary>
        /// <example>
        ///     <code>
        ///     Result&lt;int, int&gt; Sq(int x) => Result.Ok(x * x);
        ///     Result&lt;int, int&gt; Err(int x) => Result.Error(x);
        ///
        ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Sq).OrElse(Sq));
        ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Err).OrElse(Sq));
        ///     Assert.Equal(Result.Ok(9), Result.Error(3).OrElse(Sq).OrElse(Err));
        ///     Assert.Equal(Result.Error(3), Result.Error(3).OrElse(Err).OrElse(Sq));
        ///     </code>
        /// </example>
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="T">Type of the success value.</typeparam>
        /// <typeparam name="TMap">Type of the mapped error by the bound function.</typeparam>
        /// <returns>The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok" /> value.</returns>
        public Result<T, TMap> OrElse<T, TMap>(Func<TError, Result<T, TMap>> op)
            where TMap : notnull => op(Value);
    }
}
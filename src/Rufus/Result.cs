using static System.Result;

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

    /// <summary>
    /// Defines default value type implementations of <see cref="Result{T, TError}"/> variants.
    /// </summary>
    public static class Values
    {
        /// <summary>
        ///     Value type to pass a success value that can be implicitly converted to a proper
        ///     <see cref="Result{T, TError}" /> by avoiding error generic type definition.
        /// </summary>
        public readonly record struct Ok<T>(T Value) : Result.Ok<T>
            where T : notnull
        {
            /// <summary>
            ///     Binds <paramref name="fn" /> function to be executed if the result is <see cref="Ok{T}" />.
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
            /// <returns>
            /// The result of the bound function if <see cref="Ok{T}" />, same error in case of <see cref="Error{TError}" />.
            /// </returns>
            public Result<TMap, TError> AndThen<TMap, TError>(Func<T, Result<TMap, TError>> fn)
                where TError : notnull
                where TMap : notnull
                => fn(Value);

            /// <summary>
            ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{TError}" />.
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
            /// <typeparam name="TError">Type of the source result error.</typeparam>
            /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
            /// <returns>The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" /> value.</returns>
            public Result<T, TMapError> OrElse<TError, TMapError>(Func<TError, Result<T, TMapError>> op)
                where TError : notnull
                where TMapError : notnull => Result.Ok(Value);
        }

        /// <summary>
        ///     Value type to pass an error value that can be implicitly converted to a proper
        ///     <see cref="Result{T, TError}" /> by avoiding success generic type definition.
        /// </summary>
        public readonly record struct Error<TError>(TError Value) : Result.Error<TError>
            where TError : notnull
        {
            /// <summary>
            ///     Binds a function to be executed if the result is <see cref="Ok{T}" />.
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
            /// <returns>The result of the bound function if <see cref="Ok{T}" />, same error in case of <see cref="Error{TError}" />.</returns>
            public Result<TMap, TError> AndThen<T, TMap>(Func<T, Result<TMap, TError>> _)
                where TMap : notnull => Result.Error(Value);

            /// <summary>
            ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{TError}" />.
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
            /// <typeparam name="TErrorMap">Type of the mapped error by the bound function.</typeparam>
            /// <returns>The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" /> value.</returns>
            public Result<T, TErrorMap> OrElse<T, TErrorMap>(Func<TError, Result<T, TErrorMap>> op)
                where TErrorMap : notnull
                where T : notnull
                => op(Value);
        }
    }
}
// ReSharper restore InconsistentNaming
#pragma warning restore IDE1006 // Naming Styles
namespace System;

using static Result;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static class ResultSyntax
{
#if NET10_0_OR_GREATER
    /// <summary>
    ///     Result static initialization methods.
    /// </summary>
    extension(Result)
    {
        /// <summary>
        ///     Returns a success results of a void operations, containing a unit value.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Ok<_> Ok()
#else
        public static Values.Ok<_> Ok()
#endif
            => new();

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
#if NET11_0_OR_GREATER
        public static Ok<T> Ok<T>(T value)
#else
        public static Values.Ok<T> Ok<T>(T value)
#endif
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
#if NET11_0_OR_GREATER
        public static Error<TError> Error<TError>(TError value)
#else
        public static Values.Error<TError> Error<TError>(TError value)
#endif
            where TError : notnull => new(value);
    }
#endif

    /// <summary>
    ///     Unnest any <see cref="Result{T,TError}" /> that returns any result else in case of
    ///     <see cref="Ok{T}" />.
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
        where T : notnull =>
        result switch
        {
            Ok<Result<T, TError>>(var ok) => ok,
            Error<TError>(var error) => Error<T, TError>(error),
            _ => throw Uninitialized(),
        };

    private static Task<Result<TSuccess, TError>> Bind<T, TSuccess, TError>(
        this T value, Func<T, Result<TSuccess, TError>> fn)
        where TSuccess : notnull
        where TError : notnull
    {
        try
        {
            return Task.FromResult(fn(value));
        }
        catch (Exception exception)
        {
            return Task.FromException<Result<TSuccess, TError>>(exception);
        }
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Result<T, TError> result,
                                                                      Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull =>
        result switch
        {
            Ok<T>(var ok) => next(ok),
            Error<TError>(var error) => Task.FromResult(Error<TMap, TError>(error)),
            _ => throw Uninitialized(),
        };

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(
        this Result<T, TError> result, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull =>
        result switch
        {
            Ok<T>(var ok) => next(ok),
            Error<TError>(var error) => ValueTask.FromResult(Error<TMap, TError>(error)),
            _ => throw Uninitialized(),
        };

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this Result<T, TError> result, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull =>
        result switch
        {
            Ok<T>(var ok) => Task.FromResult(Ok<T, TMapError>(ok)),
            Error<TError>(var error) => op(error),
            _ => throw Uninitialized(),
        };

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this Result<T, TError> result, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull =>
        result switch
        {
            Ok<T>(var ok) => ValueTask.FromResult(Ok<T, TMapError>(ok)),
            Error<TError>(var error) => op(error),
            _ => throw Uninitialized(),
        };

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Task<Result<T, TError>> promise,
                                                                      Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                Task.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async Task<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<T, Task<Result<TMap, TError>>> next)
        {
            return await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => await next(ok).ConfigureAwait(false),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
        }
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(
        this Task<Result<T, TError>> promise, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                ValueTask.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                               Func<T, ValueTask<Result<TMap, TError>>> next) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => await next(ok).ConfigureAwait(false),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Task<Result<T, TError>> promise,
                                                                      Func<T, Result<TMap, TError>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } => ok.Bind(next),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                Task.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async Task<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<T, Result<TMap, TError>> next) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => next(ok),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this Task<Result<T, TError>> promise, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                Task.FromResult(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } => op(error),
            _ => BindAsync(promise: promise, op: op),
        };

        static async Task<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<TError, Task<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => await op(error).ConfigureAwait(false),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this Task<Result<T, TError>> promise, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                ValueTask.FromResult(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } => op(error),
            _ => BindAsync(promise: promise, op: op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                               Func<TError, ValueTask<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => await op(error).ConfigureAwait(false),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this Task<Result<T, TError>> promise, Func<TError, Result<T, TMapError>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                Task.FromResult<Result<T, TMapError>>(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } => Task.
                FromResult(op(error)),
            _ => BindAsync(promise: promise, op: op),
        };

        static async Task<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<TError, Result<T, TMapError>> op) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => op(error),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(
        this ValueTask<Result<T, TError>> promise, Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                new ValueTask<Result<TMap, TError>>(next(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                ValueTask.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, Task<Result<TMap, TError>>> next)
        {
            return await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => await next(ok).ConfigureAwait(false),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
        }
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(
        this ValueTask<Result<T, TError>> promise, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                ValueTask.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, ValueTask<Result<TMap, TError>>> next)
            => await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => await next(ok).ConfigureAwait(false),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Ok{T}" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Ok{T}" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(
        this ValueTask<Result<T, TError>> promise, Func<T, Result<TMap, TError>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                new ValueTask<Result<TMap, TError>>(ok.Bind(next)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                ValueTask.FromResult(Error<TMap, TError>(error)),
            _ => BindAsync(promise: promise, next: next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, Result<TMap, TError>> next) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => next(ok),
                Error<TError>(var error) => Error<TMap, TError>(error),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this ValueTask<Result<T, TError>> promise, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                ValueTask.FromResult<Result<T, TMapError>>(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                new ValueTask<Result<T, TMapError>>(op(error)),
            _ => BindAsync(promise: promise, op: op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, Task<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => await op(error).ConfigureAwait(false),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this ValueTask<Result<T, TError>> promise, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } => ValueTask.
                FromResult(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } => op(error),
            _ => BindAsync(promise: promise, op: op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, ValueTask<Result<T, TMapError>>> op) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => await op(error).ConfigureAwait(false),
                _ => throw Uninitialized(),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error{T}" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok{T}" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(
        this ValueTask<Result<T, TError>> promise, Func<TError, Result<T, TMapError>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Ok<T>(var ok) } =>
                ValueTask.FromResult(Ok<T, TMapError>(ok)),
            { IsCompletedSuccessfully: true, Result: Error<TError>(var error) } =>
                new ValueTask<Result<T, TMapError>>(error.Bind(op)),
            _ => BindAsync(promise: promise, op: op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, Result<T, TMapError>> op) =>
            await promise.ConfigureAwait(false) switch
            {
                Ok<T>(var ok) => Ok<T, TMapError>(ok),
                Error<TError>(var error) => op(error),
                _ => throw Uninitialized(),
            };
    }

    private static Result<TOk, TError> Ok<TOk, TError>(TOk value)
        where TOk : notnull
        where TError : notnull =>
#if NET11_0_OR_GREATER
        new Ok<TOk>(value);
#else
        new Result<TOk, TError>.Ok(value);
#endif

    private static Result<TOk, TError> Error<TOk, TError>(TError value)
        where TOk : notnull
        where TError : notnull =>
#if NET11_0_OR_GREATER
        new Error<TError>(value);
#else
        new Result<TOk, TError>.Error(value);
#endif
}
namespace System;

using System.Runtime.CompilerServices;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static class ResultSyntax
{
    /// <summary>
    ///     Unnest any <see cref="Result{T,TError}" /> that returns any result else in case of <see cref="Result{T, TError}.Ok" />.
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

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Result<T, TError> result, Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
        => result switch
        {
            Result<T, TError>.Ok(var ok) => next(ok),
            Result<T, TError>.Error(var error) => Task.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
        };

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(this Result<T, TError> result, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
        => result switch
        {
            Result<T, TError>.Ok(var ok) => next(ok),
            Result<T, TError>.Error(var error) => ValueTask.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
        };

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(this Result<T, TError> result, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
        => result switch
        {
            Result<T, TError>.Ok(var ok) => Task.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            Result<T, TError>.Error(var error) => op(error),
        };

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="result">The result to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(this Result<T, TError> result, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
        => result switch
        {
            Result<T, TError>.Ok(var ok) => ValueTask.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            Result<T, TError>.Error(var error) => op(error),
        };

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Task<Result<T, TError>> promise, Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                Task.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async Task<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<T, Task<Result<TMap, TError>>> next)
        {
            return await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
        }
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(this Task<Result<T, TError>> promise, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                ValueTask.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                               Func<T, ValueTask<Result<TMap, TError>>> next)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static Task<Result<TMap, TError>> AndThen<T, TError, TMap>(this Task<Result<T, TError>> promise, Func<T, Result<TMap, TError>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => ok.Bind(next),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                Task.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async Task<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<T, Result<TMap, TError>> next)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => next(ok),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(this Task<Result<T, TError>> promise, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                Task.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => op(error),
            _ => BindAsync(promise, op),
        };

        static async Task<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<TError, Task<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => await op(error).ConfigureAwait(false),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(this Task<Result<T, TError>> promise, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                ValueTask.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => op(error),
            _ => BindAsync(promise, op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                               Func<TError, ValueTask<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => await op(error).ConfigureAwait(false),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static Task<Result<T, TMapError>> OrElse<T, TError, TMapError>(this Task<Result<T, TError>> promise, Func<TError, Result<T, TMapError>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                Task.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => Task.FromResult<Result<T, TMapError>>(op(error)),
            _ => BindAsync(promise, op),
        };

        static async Task<Result<T, TMapError>> BindAsync(Task<Result<T, TError>> promise,
                                                          Func<TError, Result<T, TMapError>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => op(error),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(this ValueTask<Result<T, TError>> promise, Func<T, Task<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                new ValueTask<Result<TMap, TError>>(next(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                ValueTask.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, Task<Result<TMap, TError>>> next)
        {
            return await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
        }
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(this ValueTask<Result<T, TError>> promise, Func<T, ValueTask<Result<TMap, TError>>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                ValueTask.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, ValueTask<Result<TMap, TError>>> next)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
    }

    /// <summary>
    ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
    ///     order to return another result.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
    /// <returns>
    ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
    ///     another result containing the error value.
    /// </returns>
    public static ValueTask<Result<TMap, TError>> AndThen<T, TError, TMap>(this ValueTask<Result<T, TError>> promise, Func<T, Result<TMap, TError>> next)
        where T : notnull
        where TError : notnull
        where TMap : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                new ValueTask<Result<TMap, TError>>(ok.Bind(next)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                ValueTask.FromResult<Result<TMap, TError>>(new Result<TMap, TError>.Error(error)),
            _ => BindAsync(promise, next),
        };

        static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<T, Result<TMap, TError>> next)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => next(ok),
                Result<T, TError>.Error(var error) => new Result<TMap, TError>.Error(error),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(this ValueTask<Result<T, TError>> promise, Func<TError, Task<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                ValueTask.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                new ValueTask<Result<T, TMapError>>(op(error)),
            _ => BindAsync(promise, op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, Task<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => await op(error).ConfigureAwait(false),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(this ValueTask<Result<T, TError>> promise, Func<TError, ValueTask<Result<T, TMapError>>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => ValueTask.
                FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => op(error),
            _ => BindAsync(promise, op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, ValueTask<Result<T, TMapError>>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => await op(error).ConfigureAwait(false),
            };
    }

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <param name="promise">The promise to bind.</param>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TError">The error value type.</typeparam>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>
    ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
    ///     value.
    /// </returns>
    public static ValueTask<Result<T, TMapError>> OrElse<T, TError, TMapError>(this ValueTask<Result<T, TError>> promise, Func<TError, Result<T, TMapError>> op)
        where T : notnull
        where TError : notnull
        where TMapError : notnull
    {
        return promise switch
        {
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                ValueTask.FromResult<Result<T, TMapError>>(new Result<T, TMapError>.Ok(ok)),
            { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                new ValueTask<Result<T, TMapError>>(error.Bind(op)),
            _ => BindAsync(promise, op),
        };

        static async ValueTask<Result<T, TMapError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                               Func<TError, Result<T, TMapError>> op)
            => await promise.ConfigureAwait(false) switch
            {
                Result<T, TError>.Ok(var ok) => new Result<T, TMapError>.Ok(ok),
                Result<T, TError>.Error(var error) => op(error),
            };
    }

#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    private static Task<Result<TSuccess, TError>> Bind<T, TSuccess, TError>(this T value, Func<T, Result<TSuccess, TError>> fn)
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
}
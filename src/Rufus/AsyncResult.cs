namespace Rufus;

/// <summary>
///     Syntax methods to bind asynchronous results to a type, such as <see cref="Task{TResult}" /> or
///     <see cref="ValueTask{TResult}" />.
///     Instead of creating a new monad, it extends dotnet asynchronous types like Task and ValueTask, letting the decision
///     to the developer when to optimize allocations.
/// </summary>
public static  class AsyncResult
{
    private static Task<Result<TSuccess, TError>> Foo<T, TSuccess, TError>(
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

    extension<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public Task<Result<TMap, TError>> AndThen<TMap>(Func<T, Task<Result<TMap, TError>>> next)
            where TMap : notnull
            => result switch
            {
                Result<T, TError>.Ok(var ok) => next(ok),
                Result<T, TError>.Error(var error) => Task.FromError<TMap, TError>(error),
            };

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public ValueTask<Result<TMap, TError>> AndThen<TMap>(Func<T, ValueTask<Result<TMap, TError>>> next)
            where TMap : notnull
            => result switch
            {
                Result<T, TError>.Ok(var ok) => next(ok),
                Result<T, TError>.Error(var error) => ValueTask.FromError<TMap, TError>(error),
            };

        /// <summary>
        ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
        ///     This function can be used for control flow based on result values.
        /// </summary>
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public Task<Result<T, TMapError>> OrElse<TMapError>(Func<TError, Task<Result<T, TMapError>>> op)
            where TMapError : notnull
            => result switch
            {
                Result<T, TError>.Ok(var ok) => Task.FromOk<T, TMapError>(ok),
                Result<T, TError>.Error(var error) => op(error),
            };

        /// <summary>
        ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Result{T,TError}.Error" />.
        ///     This function can be used for control flow based on result values.
        /// </summary>
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public ValueTask<Result<T, TMapError>> OrElse<TMapError>(Func<TError, ValueTask<Result<T, TMapError>>> op)
            where TMapError : notnull
            => result switch
            {
                Result<T, TError>.Ok(var ok) => ValueTask.FromOk<T, TMapError>(ok),
                Result<T, TError>.Error(var error) => op(error),
            };

        #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    extension<T, TError>(Task<Result<T, TError>> promise)
        where T : notnull
        where TError : notnull
    {
        #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public Task<Result<TMap, TError>> AndThen<TMap>(Func<T, Task<Result<TMap, TError>>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => Task.
                    FromError<TMap, TError>(error),
                _ => BindAsync(promise, next),
            };

            static async Task<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                              Func<T, Task<Result<TMap, TError>>> next)
            {
                return await promise.ConfigureAwait(false) switch
                {
                    Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                    Result<T, TError>.Error(var error) => Result.Error(error),
                };
            }
        }

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public ValueTask<Result<TMap, TError>> AndThen<TMap>(Func<T, ValueTask<Result<TMap, TError>>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => ValueTask.
                    FromError<TMap, TError>(error),
                _ => BindAsync(promise, next),
            };

            static async ValueTask<Result<TMap, TError>> BindAsync(Task<Result<T, TError>> promise,
                                                                   Func<T, ValueTask<Result<TMap, TError>>> next)
                => await promise.ConfigureAwait(false) switch
                {
                    Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                    Result<T, TError>.Error(var error) => Result.Error(error),
                };
        }

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public Task<Result<TMap, TError>> AndThen<TMap>(Func<T, Result<TMap, TError>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => ok.Foo(next),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => Task.
                    FromError<TMap, TError>(error),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public Task<Result<T, TMapError>> OrElse<TMapError>(Func<TError, Task<Result<T, TMapError>>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    Task.FromOk<T, TMapError>(ok),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public ValueTask<Result<T, TMapError>> OrElse<TMapError>(Func<TError, ValueTask<Result<T, TMapError>>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => ValueTask.
                    FromOk<T, TMapError>(ok),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public Task<Result<T, TMapError>> OrElse<TMapError>(Func<TError, Result<T, TMapError>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    Task.FromOk<T, TMapError>(ok),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => error.Foo(op),
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

        #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    extension<T, TError>(ValueTask<Result<T, TError>> promise)
        where T : notnull
        where TError : notnull
    {
        #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        #pragma warning disable CA2012 // ValueTask instances returned from method calls should be directly awaited, returned, or passed as an argument to another method cal

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public ValueTask<Result<TMap, TError>> AndThen<TMap>(Func<T, Task<Result<TMap, TError>>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    new ValueTask<Result<TMap, TError>>(next(ok)),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => ValueTask.
                    FromError<TMap, TError>(error),
                _ => BindAsync(promise, next),
            };

            static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                                   Func<T, Task<Result<TMap, TError>>> next)
            {
                return await promise.ConfigureAwait(false) switch
                {
                    Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                    Result<T, TError>.Error(var error) => Result.Error(error),
                };
            }
        }

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public ValueTask<Result<TMap, TError>> AndThen<TMap>(Func<T, ValueTask<Result<TMap, TError>>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => next(ok),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => ValueTask.
                    FromError<TMap, TError>(error),
                _ => BindAsync(promise, next),
            };

            static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                                   Func<T, ValueTask<Result<TMap, TError>>> next)
                => await promise.ConfigureAwait(false) switch
                {
                    Result<T, TError>.Ok(var ok) => await next(ok).ConfigureAwait(false),
                    Result<T, TError>.Error(var error) => Result.Error(error),
                };
        }

        /// <summary>
        ///     Binds the given <paramref name="next" /> function to be executed in successful result that takes the OK value in
        ///     order to return another result.
        /// </summary>
        /// <param name="next">The function to be bound on <see cref="Result{T,TError}.Ok" />.</param>
        /// <typeparam name="TMap">The returned success value returned by given function.</typeparam>
        /// <returns>
        ///     A promise that executes <paramref name="next" /> when <see cref="Result{T,TError}.Ok" />, otherwise returns
        ///     another result containing the error value.
        /// </returns>
        public ValueTask<Result<TMap, TError>> AndThen<TMap>(Func<T, Result<TMap, TError>> next)
            where TMap : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    new ValueTask<Result<TMap, TError>>(ok.Foo(next)),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => ValueTask.
                    FromError<TMap, TError>(error),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public ValueTask<Result<T, TMapError>> OrElse<TMapError>(Func<TError, Task<Result<T, TMapError>>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    ValueTask.FromOk<T, TMapError>(ok),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public ValueTask<Result<T, TMapError>> OrElse<TMapError>(Func<TError, ValueTask<Result<T, TMapError>>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => ValueTask.
                    FromOk<T, TMapError>(ok),
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
        /// <param name="op">The bound function that handles the error result and return a new result.</param>
        /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
        /// <returns>
        ///     The result returned by <paramref name="op" /> function, otherwise returns <see cref="Result{T,TError}.Ok" />
        ///     value.
        /// </returns>
        public ValueTask<Result<T, TMapError>> OrElse<TMapError>(Func<TError, Result<T, TMapError>> op)
            where TMapError : notnull
        {
            return promise switch
            {
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } =>
                    ValueTask.FromOk<T, TMapError>(ok),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } =>
                    new ValueTask<Result<T, TMapError>>(error.Foo(op)),
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

        #pragma warning restore CA2012 // ValueTask instances returned from method calls should be directly awaited, returned, or passed as an argument to another method cal
        #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }
}
namespace Rufus;

public static partial class AsyncResult
{
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
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Ok(var ok) } => Bind(ok, next),
                { IsCompletedSuccessfully: true, Result: Result<T, TError>.Error(var error) } => ValueTask.
                    FromError<TMap, TError>(error),
                _ => BindAsync(promise, next),
            };

            static ValueTask<Result<TMap, TError>> Bind(T ok, Func<T, Result<TMap, TError>> next)
            {
                try
                {
                    return ValueTask.FromResult(next(ok));
                }
                catch (Exception exception)
                {
                    return ValueTask.FromException<Result<TMap, TError>>(exception);
                }
            }

            static async ValueTask<Result<TMap, TError>> BindAsync(ValueTask<Result<T, TError>> promise,
                                                                   Func<T, Result<TMap, TError>> next)
                => await promise switch
                {
                    Result<T, TError>.Ok(var ok) => next(ok),
                    Result<T, TError>.Error(var error) => Result.Error(error),
                };
        }

        #pragma warning restore CA2012 // ValueTask instances returned from method calls should be directly awaited, returned, or passed as an argument to another method cal
        #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }
}
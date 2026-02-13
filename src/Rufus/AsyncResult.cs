namespace Rufus;

/// <summary>
///     Syntax methods to bind asynchronous results to a type, such as <see cref="Task{TResult}" /> or
///     <see cref="ValueTask{TResult}" />.
///     Instead of creating a new monad, it extends dotnet asynchronous types like Task and ValueTask, letting the decision
///     to the developer when to optimize allocations.
/// </summary>
public static partial class AsyncResult
{
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

        #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }
}
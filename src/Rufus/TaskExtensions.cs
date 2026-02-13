namespace Rufus;

/// <summary>
///     Extension methods of <see cref="Task{TResult}" /> promises returning any <see cref="Result{T,TError}" />.
/// </summary>
public static class TaskExtensions
{
    extension(Task)
    {
        /// <summary>
        ///     Creates a completed <see cref="Task{TResult}" /> returning a <see cref="Result{T,TError}.Ok" /> variant result
        ///     containing the specified success value.
        /// </summary>
        public static Task<Result<T, TError>> FromOk<T, TError>(T ok)
            where T : notnull
            where TError : notnull
            => Task.FromResult<Result<T, TError>>(new Result<T, TError>.Ok(ok));

        /// <summary>
        ///     Creates a completed <see cref="Task{TResult}" /> returning a <see cref="Result{T,TError}.Error" /> variant result
        ///     containing the specified error value.
        /// </summary>
        public static Task<Result<T, TError>> FromError<T, TError>(TError error)
            where T : notnull
            where TError : notnull
            => Task.FromResult<Result<T, TError>>(new Result<T, TError>.Error(error));
    }
}
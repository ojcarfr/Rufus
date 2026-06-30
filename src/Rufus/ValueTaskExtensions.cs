namespace System;

/// <summary>
///     Extension methods of <see cref="ValueTask{TResult}" /> promises returning any <see cref="Result{T,TError}" />.
/// </summary>
public static class ValueTaskExtensions
{
    extension(ValueTask)
    {
        /// <summary>
        ///     Creates a completed <see cref="ValueTask{TResult}" /> returning a <see cref="Result{T,TError}.Ok" /> variant result
        ///     containing the specified success value.
        /// </summary>
        public static ValueTask<Result<T, TError>> FromOk<T, TError>(T ok)
            where T : notnull
            where TError : notnull
            => ValueTask.FromResult<Result<T, TError>>(new Result<T, TError>.Ok(ok));

        /// <summary>
        ///     Creates a completed <see cref="ValueTask{TResult}" /> returning a <see cref="Result{T,TError}.Error" /> variant
        ///     result
        ///     containing the specified error value.
        /// </summary>
        public static ValueTask<Result<T, TError>> FromError<T, TError>(TError error)
            where T : notnull
            where TError : notnull
            => ValueTask.FromResult<Result<T, TError>>(new Result<T, TError>.Error(error));
    }
}
namespace Rufus.Tests;

using Rufus.Tests.Utils;

public static class Any
{
    public static Result.Values.Ok<T> Ok<T>(T value)
        where T : notnull =>
#if NET10_0_OR_GREATER
        Result.Ok(value);
#else
        Result.Success(value);
#endif

    public static Result.Values.Error<T> Error<T>(T value)
        where T : notnull =>
#if NET10_0_OR_GREATER
        Result.Error(value);
#else
        Result.Failure(value);
#endif

    public static class Completed
    {
        public static Promise<T, TError> Promise<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => new Promise<T, TError>.Completed(result);

        public static Promise<T, TError> Promise<T, TError>(Exception exception)
            where T : notnull
            where TError : notnull
            => new Promise<T, TError>.Completed(exception);
    }

    public static class Pending
    {
        public static Promise<T, TError> Promise<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
            => new Promise<T, TError>.Pending(result);

        public static Promise<T, TError> Promise<T, TError>(Exception exception)
            where T : notnull
            where TError : notnull
            => new Promise<T, TError>.Pending(exception);
    }
}
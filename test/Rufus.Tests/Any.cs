namespace Rufus.Tests;

using Rufus.Tests.Utils;

public static class Any
{
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
namespace Rufus.Tests;

using Rufus.Tests.Utils;

public static class Any
{
#if NET11_0_OR_GREATER
    public static Result.Ok<T> Ok<T>(T value)
#else
    public static Result.Values.Ok<T> Ok<T>(T value)
#endif
        where T : notnull => new(value);

#if NET11_0_OR_GREATER
    public static Result.Error<T> Error<T>(T value)
#else
    public static Result.Values.Error<T> Error<T>(T value)
#endif
        where T : notnull => new(value);

#if NET11_0_OR_GREATER
    public static Option.Some<T> Some<T>(T value)
#else
    public static Option.Values.Some<T> Some<T>(T value)
#endif
        where T : notnull => new(value);

#if NET11_0_OR_GREATER
    public static Option.None None() => Option.None.Value;
#else
    public static Option.Values.None None() => Option.Values.None.Value;
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
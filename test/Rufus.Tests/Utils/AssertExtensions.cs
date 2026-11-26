namespace Rufus.Tests.Utils;

using Xunit.Sdk;

public static class AssertExtensions
{
    extension(Assert)
    {
        public static Result.Ok<T> IsOk<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            if (result is Result.Ok<T> ok)
            {
                return ok;
            }

            throw result.MismatchedType(typeof(Result.Ok<T>));
        }

        public static Result.Error<TError> IsError<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            if (result is Result.Error<TError> error)
            {
                return error;
            }

            throw result.MismatchedType(typeof(Result.Error<TError>));
        }

        public static Option.Some<T> IsSome<T>(Option<T> option)
            where T : notnull
        {
            if (option is Option.Some<T> some)
            {
                return some;
            }

            throw option.MismatchedType(typeof(Option.Some<T>));
        }

        public static Option.None IsNone<T>(Option<T> option)
            where T : notnull
        {
            if (option is Option.None none)
            {
                return none;
            }

            throw option.MismatchedType(typeof(Option.None));
        }
    }

    private static IsTypeException MismatchedType<T>(this T @object, Type expectedType)
    {
        var actualTypeName = @object is null ? null : ArgumentFormatter.Format(@object.GetType());

        return IsTypeException.ForMismatchedType(
            expectedTypeName: ArgumentFormatter.Format(expectedType),
            actualTypeName);
    }
}
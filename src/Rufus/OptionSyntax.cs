namespace System;

/// <summary>
///     Defines initialization and common syntax for <see cref="Option" /> values.
/// </summary>
public static class OptionSyntax
{
#if NET10_0_OR_GREATER
    extension(Option)
    {
        /// <summary>
        ///     Returns an <see cref="Option.Some{T}" /> value containing the unit value.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Option.Some<_> Some()
#else
        public static Option.Values.Some<_> Some()
#endif
            => new();

        /// <summary>
        ///     Returns an <see cref="Option.Some{T}" /> value containing the given <paramref name="value" />.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Option.Some<T> Some<T>(T value)
#else
        public static Option.Values.Some<T> Some<T>(T value)
#endif
            where T : notnull => new(value);

        /// <summary>
        ///     Returns an <see cref="Option.None" /> value indicating an optional value containing no value.
        /// </summary>
#if NET11_0_OR_GREATER
        public static Option.None None() => Option.None.Value;
#else
        public static Option.Values.None None() => Option.Values.None.Value;
#endif
    }
#endif

    /// <summary>
    ///     Converts from <see cref="Option{T}" /> of <see cref="Option{T}" /> to <see cref="Option{T}" />.
    /// </summary>
    /// <remarks>
    ///     Flattening only removes one level of nesting at a time.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Option&lt;Option&lt;int&gt;&gt; x = Option.Some(Option.Some(6));
    ///     Assert.Equal(Option.Some(6), x.Flatten());
    ///
    ///     x = Option.Some(Option&lt;int&gt;.None.Value);
    ///     Assert.Equal(Option.None(), x.Flatten());
    ///
    ///     x = Option.None();
    ///     Assert.Equal(Option.None(), x.Flatten());
    ///     </code>
    /// </example>
    /// <param name="option">The nested option to flatten.</param>
    /// <typeparam name="T">Type of the inner optional value.</typeparam>
    /// <returns>The inner <see cref="Option{T}" /> if <see cref="Option.Some{T}" />, otherwise <see cref="Option.None" />.</returns>
    public static Option<T> Flatten<T>(this Option<Option<T>> option)
        where T : notnull =>
#if NET11_0_OR_GREATER
        option is Option.Some<Option<T>>(var inner) ? inner : Option.None.Value;
#else
            option is Option.Some<Option<T>>(var inner) ? inner : Option<T>.None.Value;
#endif

    /// <summary>
    ///     Unzips an option containing a tuple of two values.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;(int, string)&gt; x = Option.Some((1, "hi"));
    ///     Assert.Equal((Option.Some(1), Option.Some("hi")), x.Unzip());
    ///
    ///     Option&lt;(byte, int)&gt; y = Option.None();
    ///     Assert.Equal((Option&lt;byte&gt;.None.Value, Option&lt;int&gt;.None.Value), y.Unzip());
    ///     </code>
    /// </example>
    /// <param name="option">The option containing a tuple.</param>
    /// <typeparam name="T1">Type of the first tuple element.</typeparam>
    /// <typeparam name="T2">Type of the second tuple element.</typeparam>
    /// <returns>
    ///     <c>(Some(a), Some(b))</c> if <paramref name="option" /> is <c>Some((a, b))</c>,
    ///     otherwise <c>(None, None)</c>.
    /// </returns>
    public static (Option<T1>, Option<T2>) Unzip<T1, T2>(this Option<(T1, T2)> option)
        where T1 : notnull
        where T2 : notnull =>
#if NET11_0_OR_GREATER
        option is Option.Some<(T1, T2)>((var a, var b))
            ? (new Option.Some<T1>(a), new Option.Some<T2>(b))
            : (Option.None.Value, Option.None.Value);
#else
            option is Option.Some<(T1, T2)>(var (a, b))
                ? (new Option<T1>.Some(a), new Option<T2>.Some(b))
                : (Option<T1>.None.Value, Option<T2>.None.Value);
#endif

    /// <summary>
    ///     Transposes an <see cref="Option{T}" /> of a <see cref="Result{T, TError}" /> into a
    ///     <see cref="Result{T, TError}" /> of an <see cref="Option{T}" />.
    /// </summary>
    /// <remarks>
    ///     <c>Some(Ok(v))</c> is mapped to <c>Ok(Some(v))</c>, <c>Some(Error(e))</c> is mapped to <c>Error(e)</c>,
    ///     and <c>None</c> is mapped to <c>Ok(None)</c>.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Option&lt;Result&lt;int, string&gt;&gt; x = Option.Some&lt;Result&lt;int, string&gt;&gt;(Result.Ok(5));
    ///     Result&lt;Option&lt;int&gt;, string&gt; y = Result.Ok&lt;Option&lt;int&gt;&gt;(Option.Some(5));
    ///     Assert.Equal(y, x.Transpose());
    ///     </code>
    /// </example>
    /// <param name="option">The option containing a result.</param>
    /// <typeparam name="T">Type of the success value.</typeparam>
    /// <typeparam name="TError">Type of the error value.</typeparam>
    /// <returns>The transposed <see cref="Result{T, TError}" /> of <see cref="Option{T}" />.</returns>
    public static Result<Option<T>, TError> Transpose<T, TError>(this Option<Result<T, TError>> option)
        where T : notnull
        where TError : notnull =>
        option switch
        {
#if NET11_0_OR_GREATER
            Option.Some<Result<T, TError>>(Result.Ok<T>(var ok)) => new Result.Ok<Option<T>>(new Option.Some<T>(ok)),
            Option.Some<Result<T, TError>>(Result.Error<TError>(var error)) => new Result.Error<TError>(error),
            _ => new Result.Ok<Option<T>>(Option.None.Value),
#else
                Option.Some<Result<T, TError>>(Result.Ok<T>(var ok)) => new Result<Option<T>, TError>.Ok(new Option<T>.Some(ok)),
                Option.Some<Result<T, TError>>(Result.Error<TError>(var error)) => new Result<Option<T>, TError>.Error(error),
                _ => new Result<Option<T>, TError>.Ok(Option<T>.None.Value),
#endif
        };

    /// <summary>
    ///     Transforms an optional sequence into an <see cref="IEnumerable{T}" />.
    ///     If <paramref name="option" /> is <see cref="Option.None" />, the resulting sequence is empty;
    ///     otherwise, the elements of the contained sequence are yielded.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;IEnumerable&lt;int&gt;&gt; o1 = Option.Some&lt;IEnumerable&lt;int&gt;&gt;([1, 2]);
    ///     Option&lt;IEnumerable&lt;int&gt;&gt; o2 = Option.None();
    ///     Assert.Equal([1, 2], o1.AsFlattenEnumerable());
    ///     Assert.Empty(o2.AsFlattenEnumerable());
    ///     </code>
    /// </example>
    /// <param name="option">The optional sequence to flatten.</param>
    /// <typeparam name="T">Type of the sequence elements.</typeparam>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> containing the elements of the inner sequence, or empty if
    ///     <see cref="Option.None" />.
    /// </returns>
    public static IEnumerable<T> AsFlattenEnumerable<T>(this Option<IEnumerable<T>> option)
    {
        if (option is not Option.Some<IEnumerable<T>>(var inner))
        {
            yield break;
        }

        foreach (T item in inner)
        {
            yield return item;
        }
    }

    /// <summary>
    ///     Transforms an optional sequence into an <see cref="IAsyncEnumerable{T}" />.
    ///     If <paramref name="option" /> is <see cref="Option.None" />, the resulting sequence is empty;
    ///     otherwise, the elements of the contained sequence are yielded.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Option&lt;IEnumerable&lt;int&gt;&gt; o1 = Option.Some&lt;IEnumerable&lt;int&gt;&gt;([1, 2]);
    ///     Option&lt;IEnumerable&lt;int&gt;&gt; o2 = Option.None();
    ///     Assert.Equal([1, 2], o1.AsFlattenAsyncEnumerable());
    ///     Assert.Empty(o2.AsFlattenAsyncEnumerable());
    ///     </code>
    /// </example>
    /// <param name="option">The optional sequence to flatten.</param>
    /// <typeparam name="T">Type of the sequence elements.</typeparam>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> containing the elements of the inner sequence, or empty if
    ///     <see cref="Option.None" />.
    /// </returns>
    public static async IAsyncEnumerable<T> AsFlattenAsyncEnumerable<T>(this Option<IAsyncEnumerable<T>> option)
    {
        if (option is not Option.Some<IAsyncEnumerable<T>>(var inner))
        {
            yield break;
        }

        await foreach (T item in inner)
        {
            yield return item;
        }
    }
}
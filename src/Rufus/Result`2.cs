namespace System;

using System.Text;

/// <summary>
///     <see cref="Result{T,TError}" /> is the type used for returning and propagating errors. It is a discriminated union
///     with the variants <see cref="Ok" />, representing success and containing a value, and <see cref="Error" />,
///     representing failure and containing an error value.
/// </summary>
/// <remarks>
///     Functions return <see cref="Result{T,TError}" /> whenever errors are expected and recoverable like explained in
///     ROP.
///     Is not expected to replace the use of exceptions for truly exceptional situations, but
///     return failures that are part of the normal domain logic.
/// </remarks>
/// <typeparam name="T">Returned type on succeed paths.</typeparam>
/// <typeparam name="TError">Returned type on failed paths.</typeparam>
public abstract record Result<T, TError>
    where T : notnull
    where TError : notnull
{
    private Result() { }

    #pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

    /// <summary>
    ///     Gets a value indicating whether the result is <see cref="Error" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(-3);
    ///     Assert.False(result.IsError);
    ///
    ///     result = Result.Error("Some error message");
    ///     Assert.True(result.IsError);
    ///     </code>
    /// </example>
    public bool IsError => this is Error;

    /// <summary>
    ///     Gets a value indicating whether the result is <see cref="Ok" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(-3);
    ///     Assert.True(result.IsOk);
    ///
    ///     result = Result.Error("Some error message");
    ///     Assert.False(result.IsOk);
    ///     </code>
    /// </example>
    public bool IsOk => this is Ok;

    /// <summary>
    ///     Checks whether the result is <see cref="Error" /> and the underlying value satisfies
    ///     the given predicate.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Error("Some error message");
    ///     Assert.True(result.IsErrorAnd(x =&gt; x == "Some error message"));
    ///
    ///     Result&lt;int, string&gt; result = Result.Error("Unexpected error");
    ///     Assert.False(result.IsErrorAnd(x =&gt; x == "Some error message"));
    ///
    ///     Result&lt;int, string&gt; result = Result.Ok(50);
    ///     Assert.False(result.IsErrorAnd(x =&gt; x == "Some error message"));
    ///     </code>
    /// </example>
    /// <param name="predicate">The expression used to evaluate the error value.</param>
    /// <returns>
    ///     <c>true</c> if the result is <see cref="Error" /> and the underlying value satisfies
    ///     the given predicate.
    /// </returns>
    public bool IsErrorAnd(Func<TError, bool> predicate) => this is Error error && predicate(error.Value);

    /// <summary>
    ///     Checks whether the result is <see cref="Ok" /> and the underlying value satisfies
    ///     the given predicate.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     Assert.True(result.IsOkAnd(x =&gt; x &gt; 0));
    ///
    ///     Result&lt;int, string&gt; result = Result.Ok(-1);
    ///     Assert.False(result.IsOkAnd(x =&gt; x &gt; 0));
    ///
    ///     result = Result.Error("Some error message");
    ///     Assert.False(result.IsOkAnd(x =&gt; x &gt; 0));
    ///     </code>
    /// </example>
    /// <param name="predicate">The expression used to evaluate the success value.</param>
    /// <returns>
    ///     <c>true</c> if the result is <see cref="Ok" /> and the underlying value inside of it
    ///     matches a predicate.
    /// </returns>
    public bool IsOkAnd(Func<T, bool> predicate) => this is Ok(var ok) && predicate(ok);

    /// <summary>
    ///     Returns a new result in case of <see cref="Ok" />.
    /// </summary>
    /// <remarks>
    ///     Arguments passed to <see cref="And{TMap}" /> are eagerly evaluated; if you are passing the result of a
    ///     function call, it is recommended to use <see cref="AndThen{TMap}" />, which is lazily evaluated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; x = Result.Ok(2);
    ///     Result&lt;string, string&gt; y = Result.Error("late error");
    ///     Assert.Equal(Result.Error("late error"), x.And(y));
    ///
    ///     x = Result.Error("early error");
    ///     y = Result.Ok("foo");
    ///     Assert.Equal(Result.Error("early error"), x.And(y));
    ///
    ///     x = Result.Error("not a 2");
    ///     y = Result.Error("late error");
    ///     Assert.Equal(Result.Error("not a 2"), x.And(y));
    ///
    ///     x = Result.Ok(2);
    ///     y = Result.Ok("different result type");
    ///     Assert.Equal(Result.Ok("different result type"), x.And(y));
    ///     </code>
    /// </example>
    /// <param name="result">The result to be returned in case of <see cref="Ok" />.</param>
    /// <typeparam name="TMap">Type of the returned success value.</typeparam>
    /// <returns>
    ///     Returns <paramref name="result" /> if the result is <see cref="Ok" />, otherwise returns the
    ///     <see cref="Error" /> value.
    /// </returns>
    public Result<TMap, TError> And<TMap>(Result<TMap, TError> result)
        where TMap : notnull
        => this is Error(var error) ? new Result<TMap, TError>.Error(error) : result;

    /// <summary>
    ///     Binds <paramref name="fn" /> function to be executed if the result is <see cref="Ok" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     static Result&lt;string, string> SqThenToString(int value)
    ///     {
    ///         checked
    ///         {
    ///             try
    ///             {
    ///                 return Result.Ok((value * value).ToString());
    ///             }
    ///             catch(OverflowException)
    ///             {
    ///                 return Result.Error("overflowed");
    ///             }
    ///         }
    ///     }
    ///
    ///     Assert.Equal(Result.Ok(4.ToString()), Result.Ok(2).AndThen(SqThenToString));
    ///     Assert.Equal(Result.Error("overflowed"), Result.Ok(1_000_000).AndThen(SqThenToString));
    ///     Assert.Equal(Result.Error("not a number"), Result.Error("not a number").AndThen((int x) => SqThenToString(x)));
    ///     </code>
    /// </example>
    /// <param name="fn">The bound function to the current result.</param>
    /// <typeparam name="TMap">The type of result returned by bound function.</typeparam>
    /// <returns>The result of the bound function if <see cref="Ok" />, same error in case of <see cref="Error" />.</returns>
    public Result<TMap, TError> AndThen<TMap>(Func<T, Result<TMap, TError>> fn)
        where TMap : notnull => this switch
    {
        Ok(var ok) => fn(ok),
        Error(var error) => new Result<TMap, TError>.Error(error),
    };

    /// <summary>
    ///     Returns the specified <paramref name="result" /> in case of <see cref="Error" />.
    /// </summary>
    /// <remarks>
    ///     Arguments passed to <see cref="Or" /> are eagerly evaluated; if you are passing the result of a function call, it
    ///     is recommended to use <see cref="OrElse" />, which is lazily evaluated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; x = Result.Ok(2);
    ///     Result&lt;int, string&gt; y = Result.Error("late error");
    ///     Assert.Equal(Result.Ok(2), x.Or(y));
    ///
    ///     x = Result.Error("early error");
    ///     y = Result.Ok(2);
    ///     Assert.Equal(Result.Ok(2), x.Or(y));
    ///
    ///     x = Result.Error("not a 2");
    ///     y = Result.Error("late error");
    ///     Assert.Equal(Result.Error("late error"), x.Or(y));
    ///
    ///     x = Result.Ok(2);
    ///     y = Result.Ok(100);
    ///     Assert.Equal(Result.Ok(2), x.Or(y));
    ///     </code>
    /// </example>
    /// <param name="result">The result returned in case of <see cref="Error" />.</param>
    /// <typeparam name="TMapError">The type of the returned error.</typeparam>
    /// <returns>
    ///     Returns <paramref name="result" /> if the result is <see cref="Error" />, otherwise returns the
    ///     <see cref="Ok" /> value.
    /// </returns>
    public Result<T, TMapError> Or<TMapError>(Result<T, TMapError> result)
        where TMapError : notnull => this is Ok(var ok) ? new Result<T, TMapError>.Ok(ok) : result;

    /// <summary>
    ///     Binds <paramref name="op" /> function to be executed if the result is <see cref="Error" />.
    ///     This function can be used for control flow based on result values.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, int&gt; Sq(int x) => Result.Ok(x * x);
    ///     Result&lt;int, int&gt; Err(int x) => Result.Error(x);
    ///
    ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Sq).OrElse(Sq));
    ///     Assert.Equal(Result.Ok(2), Result.Ok(2).OrElse(Err).OrElse(Sq));
    ///     Assert.Equal(Result.Ok(9), Result.Error(3).OrElse(Sq).OrElse(Err));
    ///     Assert.Equal(Result.Error(3), Result.Error(3).OrElse(Err).OrElse(Sq));
    ///     </code>
    /// </example>
    /// <param name="op">The bound function that handles the error result and return a new result.</param>
    /// <typeparam name="TMapError">Type of the mapped error by the bound function.</typeparam>
    /// <returns>The result returned by <paramref name="op" /> function, otherwise returns <see cref="Ok" /> value.</returns>
    public Result<T, TMapError> OrElse<TMapError>(Func<TError, Result<T, TMapError>> op)
        where TMapError : notnull
        => this switch
        {
            Ok(var ok) => new Result<T, TMapError>.Ok(ok),
            Error(var error) => op(error),
        };

    /// <summary>
    ///     Calls the specified action with the underlying success value if the result is <see cref="Ok" />.
    /// </summary>
    /// <param name="fn">The callback function used when <see cref="Ok" />.</param>
    /// <returns>Returns the original result.</returns>
    public Result<T, TError> Inspect(Action<T> fn)
    {
        if (this is Ok(var ok))
        {
            fn(ok);
        }

        return this;
    }

    /// <summary>
    ///     Calls the specified action with the underlying error value if the result is <see cref="Error" />.
    /// </summary>
    /// <param name="fn">The callback function used when <see cref="Error" />.</param>
    /// <returns>Returns the original result.</returns>
    public Result<T, TError> InspectError(Action<TError> fn)
    {
        if (this is Error(var error))
        {
            fn(error);
        }

        return this;
    }

    /// <summary>
    ///     Maps a <see cref="Result{T,TError}" /> to success value <typeparamref name="TMap" /> by
    ///     applying the given function to a contained <see cref="Ok" /> value, leaving an
    ///     <see cref="Error" /> value untouched.
    ///     This function can be used to compose the results of two functions.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;string, string&gt; source = Result.Ok("1,2,3,4,5");
    ///
    ///     Result&lt;int, string&gt; result = sut.Map(x =&gt;
    ///         x.Split(',', StringSplitOptions.RemoveEmptyEntries)
    ///             .Select(int.Parse)
    ///             .Sum());
    ///     Assert.Equal(15, Assert.IsType&lt;Result&lt;int, string&gt;.Ok&gt;(result).Value);
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the success value.</param>
    /// <typeparam name="TMap">Type of the mapped success value.</typeparam>
    /// <returns>
    ///     A new result value containing the mapped value, or the same containing the underlying
    ///     error in case of <see cref="Error" />.
    /// </returns>
    public Result<TMap, TError> Map<TMap>(Func<T, TMap> map)
        where TMap : notnull => this switch
    {
        Ok(var ok) => new Result<TMap, TError>.Ok(map(ok)),
        Error(var error) => new Result<TMap, TError>.Error(error),
    };

    /// <summary>
    ///     Maps the underlying success value in case of <see cref="Ok" /> by applying the given map
    ///     function, or returns the provided <paramref name="default" /> value in case of
    ///     <see cref="Error" />.
    /// </summary>
    /// <remarks>
    ///     Arguments passed to <see cref="MapOr" /> are eagerly evaluated; if you are passing the result
    ///     of a function call, it is recommended to use <see cref="MapOrElse" />, which is lazily
    ///     evaluated.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Result&lt;string, string&gt; result = Result.Ok("foo");
    ///     Assert.Equal(3, result.MapOr(x =&gt; x.Length, 42));
    ///
    ///     result = Result.Error("bar");
    ///     Assert.Equal(42, Assert.IsType&lt;Result&lt;int, string&gt;.Ok&gt;(result).Value);
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the success value.</param>
    /// <param name="default">The default value returned in case of <see cref="Error" />.</param>
    /// <typeparam name="TMap">Type of the mapped success value.</typeparam>
    public TMap MapOr<TMap>(Func<T, TMap> map, TMap @default) => this is Ok(var ok) ? map(ok) : @default;

    /// <summary>
    ///     Maps a <see cref="Result{T,TError}" /> to <typeparamref name="TMap" /> by applying fallback function
    ///     <paramref name="fallback" /> to a contained <see cref="Error" /> value, of function <paramref name="map" /> to a
    ///     contained <see cref="Ok" /> value.
    ///     This function can be used to unpack a successful result while handling an error.
    /// </summary>
    /// <param name="map">The function used to map a success value.</param>
    /// <param name="fallback">The function used to return a fallback value in case of error.</param>
    /// <typeparam name="TMap">Type of the mapped success value.</typeparam>
    /// <returns>A new result value containing the mapped success value, or the fallback value in case of error.</returns>
    public TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> fallback) => this switch
    {
        Ok(var ok) => map(ok),
        Error(var error) => fallback(error),
    };

    /// <summary>
    ///     Maps an underlying error value to <typeparamref name="TMapError" /> by applying the given function to a contained
    ///     <see cref="Error" /> value, leaving an <see cref="Ok" /> value untouched.
    ///     This function can be used to pass through a successful result while handling an error.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Function&lt;int, string&gt; stringify = static x =&gt; $"error code: {x}";
    ///
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     Assert.Equal(Result.Ok(2), result.MapError(stringify));
    ///
    ///     result = Result.Error(13);
    ///     Assert.Equal(Result.Error("error code: 13"), result);
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the error.</param>
    /// <typeparam name="TMapError">Type of the returning error.</typeparam>
    public Result<T, TMapError> MapError<TMapError>(Func<TError, TMapError> map)
        where TMapError : notnull
        => this switch
        {
            Ok(var ok) => new Result<T, TMapError>.Ok(ok),
            Error(var error) => new Result<T, TMapError>.Error(map(error)),
        };

    /// <summary>
    ///     Returns the contained <see cref="Ok" /> value.
    ///     Because this function may throw an exception, its use is discouraged. Exceptions are meant for unrecoverable
    ///     errors, and may abort the entire program.
    ///     Instead, prefer to use <see cref="UnwrapOr" />, <see cref="UnwrapOrElse" />, or <see cref="UnwrapOrDefault" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     Assert.Equal(2, result);
    ///
    ///     result = Result.Error("emergency failure");
    ///     Assert.Throws&lt;InvalidOperationException&gt;(() => result.Unwrap());
    ///     </code>
    /// </example>
    /// <exception cref="InvalidOperationException">
    ///     In case the result is <see cref="Error" /> with the contained error as message.
    /// </exception>
    public T Unwrap() => Unwrap(static e => new InvalidOperationException(e.ToString()));

    /// <summary>
    ///     Returns the contained <see cref="Ok" /> value or throw an exception build by given <paramref name="fn" /> function.
    ///     Because this function may throw an exception, its use is discouraged. Exceptions are meant for unrecoverable
    ///     errors, and may abort the entire program.
    ///     Instead, prefer to use <see cref="UnwrapOr" />, <see cref="UnwrapOrElse" />, or <see cref="UnwrapOrDefault" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     Assert.Equal(2, result);
    ///
    ///     result = Result.Error("emergency failure");
    ///     Assert.Throws&lt;ApplicationException&gt;(() => result.Unwrap&lt;ApplicationException&gt;
    ///             (e => new
    ///             ApplicationException($"Error received: {e}")));
    ///     </code>
    /// </example>
    /// <param name="fn">The function that builds an exception from contained error.</param>
    /// <typeparam name="TException">Type of the thrown exception in case of <see cref="Error" />.</typeparam>
    /// <exception cref="Exception">The exception build by <paramref name="fn" /> from contained error.</exception>
    public T Unwrap<TException>(Func<TError, TException> fn)
        where TException : Exception => this switch
    {
        Ok(var ok) => ok,
        Error(var error) => throw fn(error),
    };

    /// <summary>
    ///     Returns the contained <see cref="Error" /> value or throw an exception containing the <see cref="Ok" /> value.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     var exception = Assert.Throws&lt;InvalidOperationException%gt;(() => result.UnwrapError());
    ///     Assert.Equal("2", exception.Message);
    ///
    ///     result = Result.Error("emergency failure");
    ///     Assert.Equal("emergency failure", result.UnwrapError());
    ///     </code>
    /// </example>
    /// <exception cref="InvalidOperationException">An exception containing the <see cref="Ok" /> value as message.</exception>
    public TError UnwrapError() => UnwrapError(static x => new InvalidOperationException(x.ToString()));

    /// <summary>
    ///     Returns the contained <see cref="Error" /> value or throw an exception containing the <see cref="Ok" /> value.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     var exception = Assert.Throws&lt;ApplicationException%gt;(
    ///         () => result.UnwrapError&lt;ApplicationException&gt;(
    ///             x => new ApplicationException($"Unexpected OK: {x}")));
    ///     Assert.Equal("Unexpected OK: 2", exception.Message);
    ///
    ///     result = Result.Error("emergency failure");
    ///     Assert.Equal("emergency failure", result.UnwrapError());
    ///     </code>
    /// </example>
    /// <exception cref="Exception">
    ///     Throws an exception of type <typeparamref name="TException" /> in case of <see cref="Ok" />
    ///     .
    /// </exception>
    public TError UnwrapError<TException>(Func<T, TException> fn)
        where TException : Exception => this switch
    {
        Ok(var ok) => throw fn(ok),
        Error(var error) => error,
    };

    /// <summary>
    ///     Returns the contained <see cref="Ok" /> value or a provided value.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Result&lt;int, string&gt; result = Result.Ok(9);
    ///     Assert.Equal(9, result.UnwrapOr(2));
    ///
    ///     result = Result.Error("error");
    ///     Assert.Equal(2, result.UnwrapOr(2));
    ///     </code>
    /// </example>
    /// <param name="default">The value returned in case of <see cref="Error" />.</param>
    public T UnwrapOr(T @default) => this is Ok(var ok) ? ok : @default;

    /// <summary>
    ///     Returns the <see cref="Ok" /> value. If <see cref="Error" />, then it returns the <b>default</b> of
    ///     <typeparamref name="T" />.
    /// </summary>
    /// <remarks>
    ///     Take in account that, although <typeparamref name="T" /> is defined as <b>notnull</b>, this method can return
    ///     <b>null</b> when <typeparamref name="T" /> is a reference type.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     Func&lt;string, Result&lt;int, string&gt;&gt; parse =
    ///         x => int.TryParse(x, out var r) ? Result.Ok(r) : Result.Error($"Cannot parse to int: {x}");
    ///
    ///     Assert.Equal(1909, parse("1909").UnwrapOrDefault());
    ///     Assert.Equal(0, parse("1909blarg"));
    ///     </code>
    /// </example>
    public T? UnwrapOrDefault() => this is Ok(var ok) ? ok : default;

    /// <summary>
    ///     Returns the <see cref="Ok" /> value or computes it from a function in case of <see cref="Error" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Function&lt;string, int&gt; count = x => x.Length;
    ///
    ///     Result&lt;int, string&gt; result = Result.Ok(2);
    ///     Assert.Equal(2, result.UnwrapOrElse(count));
    ///
    ///     result = Result.Error("foo");
    ///     Assert.Equal(3, result.UnwrapOrElse(count));
    ///     </code>
    /// </example>
    /// <param name="op">The function that computes an optional value by taking the error.</param>
    public T UnwrapOrElse(Func<TError, T> op) => this switch
    {
        Ok(var ok) => ok,
        Error(var error) => op(error),
    };

    /// <summary>
    ///     Disregard properties on compiler generated record.
    /// </summary>
    protected virtual bool PrintMembers(StringBuilder builder) => false;

    /// <summary>
    ///     Converts the given OK value into the <see cref="Result{T,TError}.Ok" /> variant that
    ///     contains the underlying success value.
    /// </summary>
    public static implicit operator Result<T, TError>(Result.Values.Ok<T> ok) => new Ok(ok.Value);

    /// <summary>
    ///     Converts the given ERROR value into the <see cref="Result{T,TError}.Error" /> variant that contains the underlying
    ///     error value.
    /// </summary>
    public static implicit operator Result<T, TError>(Result.Values.Error<TError> error) => new Error(error.Value);

    /// <summary>
    ///     Represents the successful result of an operation, containing a value of the specified type.
    /// </summary>
    /// <param name="Value">The underlying successful value.</param>
    public sealed record Ok(T Value) : Result<T, TError>, Result.Ok<T>;

    /// <summary>
    ///     Represents a failed result containing error information of the specified type.
    /// </summary>
    /// <remarks>
    ///     Use this type to indicate an operation that did not succeed and to provide details
    ///     about the failure. The generic parameter specifies the type of error information returned.
    /// </remarks>
    /// <param name="Value">
    ///     The underlying failure value.
    ///     Cannot be null if the error type is a reference type.
    /// </param>
    public sealed record Error(TError Value) : Result<T, TError>, Result.Error<TError>;

    #pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
}
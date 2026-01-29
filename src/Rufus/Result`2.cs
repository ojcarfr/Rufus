namespace Rufus;

/// <summary>
///     Declares a type used for returning either successful or failed domain results as described
///     in ROP approach.
///     It defines a discriminated union with two possible cases:
///     <list type="bullet">
///         <item>
///             <term>
///                 <see cref="Ok" />
///             </term>
///             <description>Represents success result containing a returned value.</description>
///         </item>
///         <item>
///             <term>
///                 <see cref="Error" />
///             </term>
///             <description>Represents failure result containing an error value.</description>
///         </item>
///     </list>
/// </summary>
/// <remarks>
///     Is not expected to replace the use of exceptions for truly exceptional situations, but
///     return failures that are part of the normal domain logic.
/// </remarks>
/// <typeparam name="T">Returned type on succeed paths.</typeparam>
/// <typeparam name="TError">Returned type on failed paths.</typeparam>
public abstract record Result<T, TError> : Result where TError : notnull
{
    private Result() { }

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
    public bool IsError => this switch
    {
            Error => true,
            var _ => false,
    };

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
    public bool IsOk => this switch
    {
            Ok => true,
            var _ => false,
    };

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
    public bool IsErrorAnd(Func<TError, bool> predicate) => this switch
    {
            Error(var value) => predicate(value),
            var _ => false,
    };

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
    public bool IsOkAnd(Func<T, bool> predicate) => this switch
    {
            Ok(var value) => predicate(value),
            var _ => false,
    };

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
    public Result<TMap, TError> Map<TMap>(Func<T, TMap> map) => this switch
    {
            Ok(var value) => new Result<TMap, TError>.Ok(map(value)),
            Error(var error) => new Result<TMap, TError>.Error(error),
            var _ => throw new System.Runtime.CompilerServices.SwitchExpressionException(this),
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
    ///     Assert.Equal(3, Assert.IsType&lt;Result&lt;int, string&gt;.Ok&gt;(result).Value);
    ///
    ///     result = Result.Error("bar");
    ///     Assert.Equal(42, Assert.IsType&lt;Result&lt;int, string&gt;.Ok&gt;(result).Value);
    ///     </code>
    /// </example>
    /// <param name="map">The function used to map the success value.</param>
    /// <param name="default">The default value returned in case of <see cref="Error" />.</param>
    /// <typeparam name="TMap">Type of the mapped success value.</typeparam>
    /// <returns>
    ///     A new result value containing either the mapped success value or the
    ///     <paramref name="default" /> one in case of error.
    ///     This method always results into an <see cref="Ok" /> result.
    /// </returns>
    public Result<TMap, TError> MapOr<TMap>(Func<T, TMap> map, TMap @default) => this switch
    {
            Ok(var value) => new Result<TMap, TError>.Ok(Value: map(value)),
            Error => new Result<TMap, TError>.Ok(@default),
            var _ => throw new System.Runtime.CompilerServices.SwitchExpressionException(this),
    };

    /// <summary>
    ///     Maps a <see cref="Result{T,TError}" /> to <typeparamref name="TMap" /> by applying fallback function
    ///     <paramref name="default" /> to a contained <see cref="Error" /> value, of function <paramref name="map" /> to a
    ///     contained <see cref="Ok" /> value.
    ///     This function can be used to unpack a successful result while handling an error.
    /// </summary>
    /// <param name="map">The function used to map a success value.</param>
    /// <param name="default">The function used to return a fallback value in case of error.</param>
    /// <typeparam name="TMap">Type of the mapped success value.</typeparam>
    /// <returns>A new result value containing the mapped success value, or the fallback value in case of error.</returns>
    public Result<TMap, TError> MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> @default) => this switch
    {
            Ok(var value) => new Result<TMap, TError>.Ok(map(value)),
            Error(var error) => new Result<TMap, TError>.Ok(@default(error)),
            var _ => throw new System.Runtime.CompilerServices.SwitchExpressionException(this),
    };

    /// <summary>
    ///     Converts the given OK value into the <see cref="Result{T,TError}.Ok" /> variant that
    ///     contains the underlying success value.
    /// </summary>
    public static implicit operator Result<T, TError>(ResultSyntax.OkValue<T> ok) => new Ok(ok.Value);

    /// <summary>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result<T, TError>(ResultSyntax.ErrorValue<TError> error) => new Error(error.Value);

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
}
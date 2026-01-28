namespace Rufus;

/// <summary>
///     Declares a type used for returning either successful or failed domain results as described in
///     ROP approach.
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
///     Is not expected to replace the use of exceptions for truly exceptional situations, but return
///     failures that are part of the normal domain logic.
/// </remarks>
/// <typeparam name="T">Returned type on succeed paths.</typeparam>
/// <typeparam name="TError">Returned type on failed paths.</typeparam>
public abstract record Result<T, TError> : Result
    where TError : notnull
{
    private Result()
    {
    }

    /// <summary>
    ///     Converts the given OK value into the <see cref="Result{T,TError}.Ok" /> variant that contains
    ///     the underlying success value.
    /// </summary>
    public static implicit operator Result<T, TError>(ResultSyntax.OkValue<T> ok) =>
        new Ok(ok.Value);

    /// <summary>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Result<T, TError>(ResultSyntax.ErrorValue<TError> error) =>
        new Error(error.Value);

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

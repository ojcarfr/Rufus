namespace Rufus;

/// <summary>
///     Defines the syntax for <see cref="Result" /> creation.
/// </summary>
public static class ResultSyntax
{
    extension(Result)
    {
        /// <summary>
        ///     Returns a success value containing the specified <paramref name="value" />.
        ///     This value type should be implicitly converted to a <see cref="Result{T, TError}" />.
        /// </summary>
        /// <example>
        ///     <code>
        ///     public static Result&lt;int, string> Divide(int numerator, int denominator) =>
        ///         denominator switch
        ///         {
        ///             0 => Result.Error("Denominator cannot be zero."),
        ///             _ => Result.Ok(numerator / denominator),
        ///         };
        ///     </code>
        /// </example>
        public static OkValue<T> Ok<T>(T value) => new(value);

        /// <summary>
        ///     Returns a failure value containing the specified <paramref name="value" />.
        /// </summary>
        /// <example>
        ///     <code>
        ///     public static Result&lt;int, string> Divide(int numerator, int denominator) =>
        ///         denominator switch
        ///         {
        ///             0 => Result.Error("Denominator cannot be zero."),
        ///             _ => Result.Ok(numerator / denominator),
        ///         };
        ///     </code>
        /// </example>
        public static ErrorValue<TError> Error<TError>(TError value) => new(value);
    }

    extension<T>(Result.Ok<T> ok)
    {
        /// <summary>
        ///     Deconstructs the success result into its underlying value.
        /// </summary>
        public void Deconstruct(out T value) => value = ok.Value;
    }

    extension<TError>(Result.Error<TError> error)
    {
        /// <summary>
        ///     Deconstructs the error result into its underlying value.
        /// </summary>
        public void Deconstruct(out TError value) => value = error.Value;
    }

    /// <summary>
    ///     Value type to pass a success value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding error generic type definition.
    /// </summary>
    public readonly record struct OkValue<T>(T Value) : Result.Ok<T>;

    /// <summary>
    ///     Value type to pass an error value that can be implicitly converted to a proper
    ///     <see cref="Result{T, TError}" /> by avoiding success generic type definition.
    /// </summary>
    public readonly record struct ErrorValue<TError>(TError Value) : Result.Error<TError>;
}

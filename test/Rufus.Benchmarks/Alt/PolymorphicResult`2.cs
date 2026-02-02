namespace Rufus.Benchmarks.Alt;

public abstract record PolymorphicResult<T, TError>
    where TError : notnull
{
    private PolymorphicResult() { }

    public abstract bool IsError { get; }

    public abstract bool IsOk { get; }

    public abstract bool IsErrorAnd(Func<TError, bool> predicate);

    public abstract bool IsOkAnd(Func<T, bool> predicate);

    public abstract PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map);

    public abstract TMap MapOr<TMap>(Func<T, TMap> map, TMap @default);

    public abstract TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> fallback);

    public abstract PolymorphicResult<T, TMap> MapError<TMap>(Func<TError, TMap> map)
        where TMap : notnull;

    public static implicit operator PolymorphicResult<T, TError>(ResultSyntax.OkValue<T> ok) => new Ok(ok.Value);

    public static implicit operator PolymorphicResult<T, TError>(ResultSyntax.ErrorValue<TError> error)
        => new Error(error.Value);

    public sealed record Ok(T Value) : PolymorphicResult<T, TError>, Result.Ok<T>
    {
        /// <inheritdoc />
        public override bool IsError => false;

        /// <inheritdoc />
        public override bool IsOk => true;

        /// <inheritdoc />
        public override bool IsErrorAnd(Func<TError, bool> predicate) => false;

        /// <inheritdoc />
        public override bool IsOkAnd(Func<T, bool> predicate) => predicate(Value);

        /// <inheritdoc />
        public override PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map)
            => new PolymorphicResult<TMap, TError>.Ok(map(Value));

        /// <inheritdoc />
        public override TMap MapOr<TMap>(Func<T, TMap> map, TMap @default) => map(Value);

        /// <inheritdoc />
        public override TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> fallback) => map(Value);

        /// <inheritdoc />
        public override PolymorphicResult<T, TMap> MapError<TMap>(Func<TError, TMap> map)
            => new PolymorphicResult<T, TMap>.Ok(Value);
    }

    public sealed record Error(TError Value) : PolymorphicResult<T, TError>, Result.Error<TError>
    {
        /// <inheritdoc />
        public override bool IsError => true;

        /// <inheritdoc />
        public override bool IsOk => false;

        /// <inheritdoc />
        public override bool IsErrorAnd(Func<TError, bool> predicate) => predicate(Value);

        /// <inheritdoc />
        public override bool IsOkAnd(Func<T, bool> predicate) => false;

        /// <inheritdoc />
        public override PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map)
            => new PolymorphicResult<TMap, TError>.Error(Value);

        /// <inheritdoc />
        public override TMap MapOr<TMap>(Func<T, TMap> map, TMap @default) => @default;

        /// <inheritdoc />
        public override TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> fallback) => fallback(Value);

        /// <inheritdoc />
        public override PolymorphicResult<T, TMap> MapError<TMap>(Func<TError, TMap> map)
            => new PolymorphicResult<T, TMap>.Error(map(Value));
    }
}
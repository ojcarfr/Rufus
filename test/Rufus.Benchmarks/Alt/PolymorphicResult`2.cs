namespace Rufus.Benchmarks.Alt;

public abstract record PolymorphicResult<T, TError>
    where TError : notnull
    where T : notnull
{
    private PolymorphicResult() { }

    public abstract bool IsError { get; }

    public abstract bool IsOk { get; }

    public abstract bool IsErrorAnd(Func<TError, bool> predicate);

    public abstract bool IsOkAnd(Func<T, bool> predicate);

    public abstract PolymorphicResult<T, TError> Inspect(Action<T> fn);

    public abstract PolymorphicResult<T, TError> InspectError(Action<TError> fn);

    public abstract PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map)
        where TMap : notnull;

    public abstract TMap MapOr<TMap>(Func<T, TMap> map, TMap @default);

    public abstract TMap MapOrElse<TMap>(Func<T, TMap> map, Func<TError, TMap> fallback);

    public abstract PolymorphicResult<T, TMap> MapError<TMap>(Func<TError, TMap> map)
        where TMap : notnull;

    public static implicit operator PolymorphicResult<T, TError>(Result.Values.Ok<T> ok) => new Ok(ok.Value);

    public static implicit operator PolymorphicResult<T, TError>(Result.Values.Error<TError> error)
        => new Error(error.Value);

    public sealed record Ok(T Value) : PolymorphicResult<T, TError>, Ok<T>
    {
        /// <inheritdoc />
        public override bool IsError => false;

        /// <inheritdoc />
        public override bool IsOk => true;

        /// <inheritdoc />
        public override bool IsErrorAnd(Func<TError, bool> predicate) => false;

        /// <inheritdoc />
        public override bool IsOkAnd(Func<T, bool> predicate) => predicate(Value);

        public override PolymorphicResult<T, TError> Inspect(Action<T> fn)
        {
            fn(Value);

            return this;
        }

        public override PolymorphicResult<T, TError> InspectError(Action<TError> fn) => this;

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

    public sealed record Error(TError Value) : PolymorphicResult<T, TError>, Error<TError>
    {
        /// <inheritdoc />
        public override bool IsError => true;

        /// <inheritdoc />
        public override bool IsOk => false;

        /// <inheritdoc />
        public override bool IsErrorAnd(Func<TError, bool> predicate) => predicate(Value);

        /// <inheritdoc />
        public override bool IsOkAnd(Func<T, bool> predicate) => false;

        public override PolymorphicResult<T, TError> Inspect(Action<T> fn) => this;

        public override PolymorphicResult<T, TError> InspectError(Action<TError> fn)
        {
            fn(Value);

            return this;
        }

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
namespace Rufus.Benchmarks;

internal abstract record PolymorphicResult<T, TError>
    where TError : notnull
{
    private PolymorphicResult() { }

    public abstract bool IsOk { get; }

    public abstract PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map);

    public sealed record Ok(T Value) : PolymorphicResult<T, TError>, Result.Ok<T>
    {
        public override bool IsOk  => true;

        public override PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map)
            => new PolymorphicResult<TMap, TError>.Ok(map(this.Value));
    }

    public sealed record Error(TError Value) : PolymorphicResult<T, TError>, Result.Error<TError>
    {
        public override bool IsOk => false;

        public override PolymorphicResult<TMap, TError> Map<TMap>(Func<T, TMap> map)
            => new PolymorphicResult<TMap, TError>.Error(this.Value);
    }
}
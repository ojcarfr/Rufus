namespace Rufus.Tests.Utils;

public abstract class Promise<T, TError>
    where T : notnull
    where TError : notnull
{
    public abstract Task<Result<T, TError>> AsTask();

    public abstract ValueTask<Result<T, TError>> AsValueTask();

    public abstract void Ready();

    public sealed class Completed : Promise<T, TError>
    {
        private readonly Func<Result<T, TError>> _result;

        public Completed(Result<T, TError> result) => _result = () => result;

        public Completed(Exception exception) => _result = () => throw exception;

        public override Task<Result<T, TError>> AsTask()
        {
            try
            {
                return Task.FromResult(_result());
            }
            catch (Exception exception)
            {
                return Task.FromException<Result<T, TError>>(exception);
            }
        }

        public override ValueTask<Result<T, TError>> AsValueTask()
        {
            try
            {
                return ValueTask.FromResult(_result());
            }
            catch (Exception exception)
            {
                return ValueTask.FromException<Result<T, TError>>(exception);
            }
        }

        public override void Ready() { }
    }

    public sealed class Pending : Promise<T, TError>
    {
        private readonly Func<Result<T, TError>> _result;
        private readonly TaskCompletionSource<Result<T, TError>> _tcs = new();

        public Pending(Result<T, TError> result) => _result = () => result;

        public Pending(Exception exception) => _result = () => throw exception;

        public override Task<Result<T, TError>> AsTask() => _tcs.Task;

        public override ValueTask<Result<T, TError>> AsValueTask() => new(_tcs.Task);

        public override void Ready()
        {
            try
            {
                _tcs.TrySetResult(_result());
            }
            catch (Exception exception)
            {
                _tcs.TrySetException(exception);
            }
        }
    }
}
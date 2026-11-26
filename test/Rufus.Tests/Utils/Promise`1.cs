namespace Rufus.Tests.Utils;

public abstract class Promise<T>
    where T : notnull
{
    public abstract Task<Option<T>> AsTask();

    public abstract ValueTask<Option<T>> AsValueTask();

    public abstract void Ready();

    public sealed class Completed : Promise<T>
    {
        private readonly Func<Option<T>> _option;

        public Completed(Option<T> option) => _option = () => option;

        public Completed(Exception exception) => _option = () => throw exception;

        public override Task<Option<T>> AsTask()
        {
            try
            {
                return Task.FromResult(_option());
            }
            catch (Exception exception)
            {
                return Task.FromException<Option<T>>(exception);
            }
        }

        public override ValueTask<Option<T>> AsValueTask()
        {
            try
            {
                return ValueTask.FromResult(_option());
            }
            catch (Exception exception)
            {
                return ValueTask.FromException<Option<T>>(exception);
            }
        }

        public override void Ready() { }
    }

    public sealed class Pending : Promise<T>
    {
        private readonly Func<Option<T>> _option;
        private readonly TaskCompletionSource<Option<T>> _tcs = new();

        public Pending(Option<T> option) => _option = () => option;

        public Pending(Exception exception) => _option = () => throw exception;

        public override Task<Option<T>> AsTask() => _tcs.Task;

        public override ValueTask<Option<T>> AsValueTask() => new(_tcs.Task);

        public override void Ready()
        {
            try
            {
                _tcs.TrySetResult(_option());
            }
            catch (Exception exception)
            {
                _tcs.TrySetException(exception);
            }
        }
    }
}

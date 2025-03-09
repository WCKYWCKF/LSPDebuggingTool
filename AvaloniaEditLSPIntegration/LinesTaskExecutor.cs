using System.Collections.Concurrent;

namespace AvaloniaEditLSPIntegration;

file class LinesTaskExecutor : ILinesTaskExecutor
{
    private readonly Mutex _mutex = new();
    private readonly ConcurrentQueue<Action> _taskQueue = new();
    private Task? _task;

    public void Post(Action task)
    {
        _taskQueue.Enqueue(task);
        _mutex.WaitOne();
        _task ??= Task.Run(Execute);
        _mutex.ReleaseMutex();
    }

    private void Execute()
    {
        while (_taskQueue.TryDequeue(out var action))
            action.Invoke();
        _mutex.WaitOne();
        _task = _taskQueue.IsEmpty ? null : Task.Run(Execute);
        _mutex.ReleaseMutex();
    }

    ~LinesTaskExecutor()
    {
        _mutex.Dispose();
    }
}

public interface ILinesTaskExecutor
{
    void Post(Action task);

    public static ILinesTaskExecutor Create()
    {
        return new LinesTaskExecutor();
    }
}
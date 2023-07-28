using GameDrive.Server.Tasks;

namespace GameDrive.Server.Services;

public interface IRepeatingTaskService
{
    void Start();
    void Stop();
}

public class RepeatingTaskService : IRepeatingTaskService
{
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;

    public RepeatingTaskService(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }

    public void Start()
    {
        using var scope = _serviceProvider.CreateScope();
        var repeatingTasks = GetRepeatingTasks(scope.ServiceProvider);
        var nextInterval = TimeUntilNextRepeatingTask(repeatingTasks);
        _timer = new Timer(async o => await ProcessTasks(o), null, nextInterval, TimeSpan.Zero);
    }

    public void Stop()
    {
        _timer.Dispose();
    }

    private IReadOnlyList<RepeatingTaskWrapper> GetRepeatingTasks(IServiceProvider scopedProvider)
    {
        var repeatingTasks = scopedProvider
            .GetServices<IRepeatingTask>()
            .Select(x => new RepeatingTaskWrapper(x))
            .ToList();

        return repeatingTasks;
    }

    private async Task ProcessTasks(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var repeatingTasks = GetRepeatingTasks(scope.ServiceProvider)
            .Where(x => x.LastExecuted.Add(x.RepeatingTask.Interval).CompareTo(DateTime.Now) <= 0)
            .ToList();

        var tasks = repeatingTasks
            .Select(x => x.RepeatingTask.ExecuteAsync());
        await Task.WhenAll(tasks);

        foreach (var task in repeatingTasks)
        {
            task.LastExecuted = DateTime.Now;
        }

        var nextInterval = TimeUntilNextRepeatingTask(repeatingTasks);
        _timer.Change(nextInterval, TimeSpan.Zero);
    }

    private TimeSpan TimeUntilNextRepeatingTask(IEnumerable<RepeatingTaskWrapper> repeatingTasks)
    {
        var nextTask = repeatingTasks.MinBy(x => x.LastExecuted);
        if (nextTask is null)
            return TimeSpan.FromSeconds(30);
        
        var timeRemaining = nextTask.LastExecuted.Add(nextTask.RepeatingTask.Interval).Subtract(DateTime.Now);
        return timeRemaining.TotalMilliseconds < 0 
            ? TimeSpan.Zero 
            : timeRemaining;
    }

    private class RepeatingTaskWrapper
    {
        public IRepeatingTask RepeatingTask { get; }
        public DateTime LastExecuted { get; set; } = DateTime.MinValue;

        public RepeatingTaskWrapper(IRepeatingTask repeatingTask)
        {
            RepeatingTask = repeatingTask;
        }
    }
}
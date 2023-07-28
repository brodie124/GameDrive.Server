using GameDrive.Server.Services;

namespace GameDrive.Server.Tasks.Startup;

public class InitialiseRepeatingTaskService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public InitialiseRepeatingTaskService(
        IServiceProvider serviceProvider    
    )
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var repeatingTaskService = _serviceProvider.GetRequiredService<IRepeatingTaskService>();
        repeatingTaskService.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var repeatingTaskService = _serviceProvider.GetRequiredService<IRepeatingTaskService>();
        repeatingTaskService.Stop();
        return Task.CompletedTask;
    }
}
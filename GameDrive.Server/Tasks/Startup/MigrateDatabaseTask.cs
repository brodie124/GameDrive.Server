using GameDrive.Server.Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace GameDrive.Server.Tasks.Startup;

public class MigrateDatabaseTask : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrateDatabaseTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameDriveDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
using GameDrive.Server.Services.Storage;

namespace GameDrive.Server.Tasks.Repeating;

public class TriggerReplicationRepeatingTask : IRepeatingTask
{
    private readonly IStorageReplicationService _storageReplicationService;
    public TimeSpan Interval => TimeSpan.FromSeconds(30);

    public TriggerReplicationRepeatingTask(
        IStorageReplicationService storageReplicationService    
    )
    {
        _storageReplicationService = storageReplicationService;
    }
    
    public async Task ExecuteAsync()
    {
        await _storageReplicationService.RefreshReplicationQueue();
        await _storageReplicationService.ProcessReplicationQueue();
    }
}
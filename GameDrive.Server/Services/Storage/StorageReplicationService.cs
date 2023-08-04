using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Services.Storage;

public interface IStorageReplicationService
{
    Task ProcessReplicationQueue();
    Task RefreshReplicationQueue(CancellationToken cancellationToken = default);
}

public class StorageReplicationService : IStorageReplicationService
{
    private const int QueueSize = 128;
    private readonly ILogger<StorageReplicationService> _logger;
    private readonly IStorageObjectRepository _storageObjectRepository;
    private readonly ICloudStorageProvider _cloudStorageProvider;

    private readonly List<ReplicationQueueItem> _replicationQueue = new List<ReplicationQueueItem>();
    private readonly SemaphoreSlim _replicationSemaphoreSlim = new SemaphoreSlim(1);

    public StorageReplicationService(
        ILogger<StorageReplicationService> logger,
        IStorageObjectRepository storageObjectRepository,
        ICloudStorageProvider cloudStorageProvider
    )
    {
        _logger = logger;
        _storageObjectRepository = storageObjectRepository;
        _cloudStorageProvider = cloudStorageProvider;
    }

    public async Task ProcessReplicationQueue()
    {
        await _replicationSemaphoreSlim.WaitAsync();
        try
        {
            await ReplicateStorageObjects(_replicationQueue);

        }
        finally
        {
            _replicationSemaphoreSlim.Release();
        }
    }

    public async Task RefreshReplicationQueue(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refreshing replication queue");
        var objectsPendingReplication =
            (await _storageObjectRepository.FindAsync(x => x.TemporaryFileKey != null))
            .ToList()
            .OrderBy(x => x.UploadedDate)
            .Take(QueueSize);

        var replicationQueueItems = objectsPendingReplication.Select(x => new ReplicationQueueItem(x.Id));
        await _replicationSemaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            _replicationQueue.Clear();
            _replicationQueue.AddRange(replicationQueueItems);
        }
        finally
        {
            _replicationSemaphoreSlim.Release();
        }
        
        _logger.LogInformation("Finished refreshing replication queue");
    }

    private async Task ReplicateStorageObjects(List<ReplicationQueueItem> queueItems)
    {
        if (queueItems.Count <= 0)
        {
            _logger.LogInformation("Skipping storage object replication - queue empty");
            return;
        }
        
        _logger.LogInformation("Starting storage object replication");
        _logger.LogInformation("- Fetching game objects");
        var storageObjectIds = queueItems.Select(x => x.StorageObjectId).ToArray();
        var storageObjects = await _storageObjectRepository
            .FindAsync(x => storageObjectIds.Contains(x.Id));

        _logger.LogInformation("- Uploading objects to the cloud provider");
        var result = await _cloudStorageProvider.SaveObjectsAsync(storageObjects);
        var failedCount = result.Count(x => !x.Success);
        if (failedCount > 0)
            _logger.LogError("Failed to {Length} replicate storage objects", failedCount);

        _logger.LogInformation("- Updating the records in the database");
        foreach (var obj in storageObjects)
        {
            var saveResult = result.First(x => x.StorageObjectId == obj.Id);
            if (!saveResult.Success)
                continue;
            
            // TODO: track replication date/time
            obj.TemporaryFileKey = null;
            await _storageObjectRepository.UpdateAsync(obj);
        }

        await _storageObjectRepository.SaveChangesAsync();
        _logger.LogInformation("Finished storage object replication");
    }
}

public record ReplicationQueueItem(
    Guid StorageObjectId
);
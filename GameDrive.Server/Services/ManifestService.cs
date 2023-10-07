using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Services;

public class ManifestService
{
    private readonly ILogger<ManifestService> _logger;
    private readonly IStorageObjectRepository _storageObjectRepository;

    public ManifestService(
        ILogger<ManifestService> logger,
        IStorageObjectRepository storageObjectRepository
    )
    {
        _logger = logger;
        _storageObjectRepository = storageObjectRepository;
    }

    public async Task<CompareManifestResponse> GenerateComparisonReport(int userId, GameProfileManifest gameProfileManifest)
    {
        if (gameProfileManifest.Entries.Count != gameProfileManifest.Entries.DistinctBy(x => x.RelativePath).Count())
            throw new InvalidDataException("File manifest cannot contain duplicated entries!");
        
        var storageObjectComparisonQueue = (await _storageObjectRepository
                .FindAsync(x => x.OwnerId == userId && x.BucketId == gameProfileManifest.GameProfileId))
            .OrderByDescending(x => x.UploadedDate)
            .DistinctBy(x => x.ClientRelativePath)
            .ToList();
        
        var manifestFileReports = new List<CompareManifestResponseEntry>();
        var longestQueue = Math.Max(storageObjectComparisonQueue.Count, gameProfileManifest.Entries.Count);

        for (var i = 0; i < longestQueue; i++)
        {
            var entry = gameProfileManifest.Entries.ElementAtOrDefault(i);
            var storageObject = entry is not null
                ? storageObjectComparisonQueue.FirstOrDefault(x => x.ClientRelativePath == entry.RelativePath)
                : storageObjectComparisonQueue.FirstOrDefault();
            
            if(storageObject is not null)
                storageObjectComparisonQueue.Remove(storageObject);
            
            var fileReport = await CompareManifestEntryAsync(entry, storageObject);
            manifestFileReports.Add(fileReport);
        }
        
        return new CompareManifestResponse(manifestFileReports);
    }

    private async Task<CompareManifestResponseEntry> CompareManifestEntryAsync(ManifestEntry? entry, StorageObject? storageObject)
    {
        var crossReferenceId = entry?.Guid ?? Guid.Empty;

        // If the file's last modified date is before the previously known value (on the client side) then maybe the user
        // is intentionally replacing the file with an older value - we should report this as a conflict
        if(entry?.ClientPreviousLastModifiedDate?.CompareTo(entry.LastModifiedDate) > 0) 
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict);

        if ((entry is null || entry.IsDeleted) && (storageObject is null || storageObject.IsDeleted))
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed);
        
        // If the storage object does not exist theTn the client-side file must be new
        if (storageObject is null)
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.New)
                .WithEntry(entry);
        
        // If the client-side entry does not exist then it must be either missing or deleted on the client
        if (entry is null)
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.DownloadAdvised, FileDiffState.Missing)
                .WithStorageObject(storageObject);
            
        if (storageObject.IsDeleted)
        {
            if (storageObject.DeletedDate is null)
            {
                // StorageObject.DeletedDate should NEVER be null if StorageObject.IsDeleted is true - what happened?
                _logger.LogError("StorageObject.DeletedDate is null for StorageObject ID {StorageObjectId}", storageObject.Id);
                return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict)
                    .WithStorageObject(storageObject);
            }
            
            return entry.LastModifiedDate.CompareTo(storageObject.DeletedDate) switch
            {
                < 0 => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed).WithStorageObject(storageObject), // Last modified BEFORE the deleted date, no need to upload
                > 0 => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.Newer).WithStorageObject(storageObject), // Last modified AFTER the deleted date, client-side must be newer
                _ => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict).WithStorageObject(storageObject), // Removed at exactly the same time as it was updated - raise a conflict.
            };
        }

        if (entry.IsDeleted)
        {
            storageObject.MarkForDeletion();
            await _storageObjectRepository.UpdateAsync(storageObject);
            await _storageObjectRepository.SaveChangesAsync();
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed);
        }
        
        if (entry.FileHash == storageObject.FileHash)
            return new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Ignore, FileDiffState.Same).WithStorageObject(storageObject);
        
        return entry.LastModifiedDate.CompareTo(storageObject.LastModifiedDate) switch
        {
            > 0 => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.Newer).WithStorageObject(storageObject),
            < 0 => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.DownloadAdvised, FileDiffState.Older).WithStorageObject(storageObject),
            _ => new CompareManifestResponseEntry(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict).WithStorageObject(storageObject)
        };
    }
}
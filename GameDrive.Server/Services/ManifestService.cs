using GameDrive.Server.Domain.Models;
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

    public async Task<List<ManifestFileReport>> GenerateComparisonReport(int userId, FileManifest fileManifest)
    {
        if (fileManifest.Entries.Count != fileManifest.Entries.DistinctBy(x => x.RelativePath).Count())
            throw new InvalidDataException("File manifest cannot contain duplicated entries!");
        
        var storageObjectComparisonQueue = (await _storageObjectRepository
                .FindAsync(x => x.OwnerId == userId && x.BucketId == fileManifest.BucketId))
            .OrderByDescending(x => x.UploadedDate)
            .DistinctBy(x => x.ClientRelativePath)
            .ToList();
        
        var manifestFileReports = new List<ManifestFileReport>();
        var longestQueue = Math.Max(storageObjectComparisonQueue.Count, fileManifest.Entries.Count);

        for (var i = 0; i < longestQueue; i++)
        {
            var entry = fileManifest.Entries.ElementAtOrDefault(i);
            var storageObject = entry is not null
                ? storageObjectComparisonQueue.FirstOrDefault(x => x.ClientRelativePath == entry.RelativePath)
                : storageObjectComparisonQueue.FirstOrDefault();
            
            if(storageObject is not null)
                storageObjectComparisonQueue.Remove(storageObject);
            
            var fileReport = await CompareManifestEntryAsync(entry, storageObject);
            manifestFileReports.Add(fileReport);
        }
        
        return manifestFileReports;
    }

    private async Task<ManifestFileReport> CompareManifestEntryAsync(ManifestEntry? entry, StorageObject? storageObject)
    {
        var crossReferenceId = entry?.Guid ?? Guid.Empty;

        if ((entry is null || entry.IsDeleted) && (storageObject is null || storageObject.IsDeleted))
            return new ManifestFileReport(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed, null);
        
        // If the storage object does not exist then the client-side file must be new
        if (storageObject is null)
            return new ManifestFileReport(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.New, null);
        
        // If the client-side entry does not exist then it must be either missing or deleted on the client
        if (entry is null)
            return new ManifestFileReport(crossReferenceId, FileUploadState.DownloadAdvised, FileDiffState.Missing, storageObject);
            
        if (storageObject.IsDeleted)
        {
            if (storageObject.DeletedDate is null)
            {
                // StorageObject.DeletedDate should NEVER be null if StorageObject.IsDeleted is true - what happened?
                _logger.LogError("StorageObject.DeletedDate is null for StorageObject ID {StorageObjectId}", storageObject.Id);
                return new ManifestFileReport(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict, storageObject);
            }
            
            return entry.LastModifiedDate.CompareTo(storageObject.DeletedDate) switch
            {
                < 0 => new ManifestFileReport(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed, storageObject), // Last modified BEFORE the deleted date, no need to upload
                > 0 => new ManifestFileReport(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.Newer, storageObject), // Last modified AFTER the deleted date, client-side must be newer
                _ => new ManifestFileReport(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict, storageObject), // Removed at exactly the same time as it was updated - raise a conflict.
            };
        }

        if (entry.IsDeleted)
        {
            storageObject.MarkForDeletion();
            await _storageObjectRepository.UpdateAsync(storageObject);
            return new ManifestFileReport(crossReferenceId, FileUploadState.Ignore, FileDiffState.Removed, null);
        }
        
        if (entry.FileHash == storageObject.FileHash)
            return new ManifestFileReport(crossReferenceId, FileUploadState.Ignore, FileDiffState.Same, storageObject);
        
        return entry.LastModifiedDate.CompareTo(storageObject.LastModifiedDate) switch
        {
            > 0 => new ManifestFileReport(crossReferenceId, FileUploadState.UploadRequested, FileDiffState.Newer, storageObject),
            < 0 => new ManifestFileReport(crossReferenceId, FileUploadState.DownloadAdvised, FileDiffState.Older, storageObject),
            _ => new ManifestFileReport(crossReferenceId, FileUploadState.Conflict, FileDiffState.Conflict, storageObject)
        };
    }
}
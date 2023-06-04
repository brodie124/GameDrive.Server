using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Services;

public class ManifestService
{
    private readonly ILogger<ManifestService> _logger;
    private readonly StorageObjectRepository _storageObjectRepository;

    public ManifestService(
        ILogger<ManifestService> logger,
        StorageObjectRepository storageObjectRepository
    )
    {
        _logger = logger;
        _storageObjectRepository = storageObjectRepository;
    }

    public async Task<List<ManifestFileReport>> GenerateComparisonReport(int userId, FileManifest fileManifest)
    {
        var storageObjectComparisonQueue = (await _storageObjectRepository
                .FindAsync(x => x.OwnerId == userId && x.BucketId == fileManifest.BucketId))
            .OrderByDescending(x => x.UploadedDate)
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
            
            var fileReport = CompareManifestEntry(entry, storageObject);
            manifestFileReports.Add(fileReport);
        }
        
        return manifestFileReports;
    }

    private ManifestFileReport CompareManifestEntry(ManifestEntry? entry, StorageObject? storageObject)
    {
        var crossReferenceId = entry?.Guid ?? Guid.Empty;
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
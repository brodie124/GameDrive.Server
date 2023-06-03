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

    public async Task<Dictionary<Guid, ManifestFileReport>> GenerateComparisonReport(int userId, FileManifest fileManifest)
    {
        var storageObjects = (await _storageObjectRepository
                .FindAsync(x => x.OwnerId == userId && x.BucketId == fileManifest.BucketId))
            .OrderByDescending(x => x.UploadedDate)
            .ToList();
        var manifestFileReports = new Dictionary<Guid, ManifestFileReport>();
        foreach (var entry in fileManifest.Entries)
        {
            var storageObj = storageObjects.FirstOrDefault(x => x.FileNameWithExtension == entry.FileName);
            var fileReport = CompareManifestEntry(entry, storageObj);
            manifestFileReports.Add(entry.Guid, fileReport);
        }

        return manifestFileReports;
    }

    private ManifestFileReport CompareManifestEntry(ManifestEntry entry, StorageObject? storageObject)
    {
        // If the storage object does not exist then the client-side file must be new
        if (storageObject is null)
            return new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.New, null);

        if (storageObject.IsDeleted)
        {
            if (storageObject.DeletedDate is null)
            {
                // StorageObject.DeletedDate should NEVER be null if StorageObject.IsDeleted is true - what happened?
                _logger.LogError("StorageObject.DeletedDate is null for StorageObject ID {StorageObjectId}", storageObject.Id);
                return new ManifestFileReport(FileUploadState.Conflict, FileDiffState.Conflict, storageObject);
            }
            
            return entry.LastModifiedDate.CompareTo(storageObject.DeletedDate) switch
            {
                < 0 => new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Removed, storageObject), // Last modified BEFORE the deleted date, no need to upload
                > 0 => new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.Newer, storageObject), // Last modified AFTER the deleted date, client-side must be newer
                _ => new ManifestFileReport(FileUploadState.Conflict, FileDiffState.Conflict, storageObject), // Removed at exactly the same time as it was updated - raise a conflict.
            };
        }
        
        if (entry.FileHash == storageObject.FileHash)
            return new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Same, storageObject);
        
        return entry.LastModifiedDate.CompareTo(storageObject.LastModifiedDate) switch
        {
            > 0 => new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.Newer, storageObject),
            < 0 => new ManifestFileReport(FileUploadState.DownloadAdvised, FileDiffState.Older, storageObject),
            _ => new ManifestFileReport(FileUploadState.Conflict, FileDiffState.Conflict, storageObject)
        };
    }
}
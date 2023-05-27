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
                .FindAsync(x => x.OwnerId == userId && x.GameProfileId == fileManifest.GameProfileId))
            .OrderByDescending(x => x.UploadedDate)
            .ToList();
        var manifestFileReports = new Dictionary<Guid, ManifestFileReport>();
        foreach (var entry in fileManifest.Entries)
        {
            var storageObj = storageObjects.FirstOrDefault(x => x.FileName == entry.FileName);
            var fileReport = CompareManifestEntry(entry, storageObj);
            manifestFileReports.Add(entry.Guid, fileReport);
        }

        return manifestFileReports;
    }

    private ManifestFileReport CompareManifestEntry(ManifestEntry entry, StorageObject? storageObject)
    {
        if (storageObject is null)
            return new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.New);
        
        if (entry.FileHash == storageObject.FileHash)
            return new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Same);

        return entry.LastModifiedDate.CompareTo(storageObject.LastModifiedDate) switch
        {
            > 0 => new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.Newer),
            < 0 => new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Older),
            _ => new ManifestFileReport(FileUploadState.Conflict, FileDiffState.Conflict)
        };
    }
}
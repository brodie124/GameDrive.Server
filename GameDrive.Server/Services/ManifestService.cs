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
        var t = new Dictionary<Guid, ManifestFileReport>();
        foreach (var entry in fileManifest.Entries)
        {
            var storageObj = storageObjects.FirstOrDefault(x => x.FileName == entry.FileName);
            if (storageObj is null)
            {
                // t.Add(entry.Guid, ManifestComparisonResult.New);
                t.Add(entry.Guid, new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.New));
                continue;
            }
            
            if (entry.FileHash == storageObj.FileHash)
            {
                // t.Add(entry.Guid, ManifestComparisonResult.Same);
                t.Add(entry.Guid, new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Same));
                continue;
            }

            switch (entry.LastModifiedDate.CompareTo(storageObj.LastModifiedDate))
            {
                case > 0:
                    // t.Add(entry.Guid, ManifestComparisonResult.Newer);
                    t.Add(entry.Guid, new ManifestFileReport(FileUploadState.UploadRequested, FileDiffState.Newer));
                    continue;
                case < 0:
                    // t.Add(entry.Guid, ManifestComparisonResult.Older);
                    t.Add(entry.Guid, new ManifestFileReport(FileUploadState.Ignore, FileDiffState.Older));
                    continue;
            }
        }

        return t;
    }
}
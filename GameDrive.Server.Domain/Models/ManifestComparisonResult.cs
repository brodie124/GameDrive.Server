namespace GameDrive.Server.Domain.Models;

public record ManifestFileReport(
    FileUploadState UploadState,
    FileDiffState DiffState
);

public enum FileUploadState
{
    UploadRequested,
    Ignore
}

public enum FileDiffState
{
    New,
    Newer,
    Older,
    Removed,
    Same
}
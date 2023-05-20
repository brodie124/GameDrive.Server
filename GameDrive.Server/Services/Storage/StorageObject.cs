namespace GameDrive.Server.Services.Storage;

// TODO: could de-duplicate files when they are uploaded, in case two people have the same file?
public record StorageObject(
    string ClientPath,
    string GameDrivePath,
    Guid Id
);
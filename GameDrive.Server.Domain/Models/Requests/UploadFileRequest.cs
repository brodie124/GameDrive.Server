namespace GameDrive.Server.Domain.Models.Requests;

public record UploadFileRequest(
    string MultiPartName,
    string BucketId,
    string BucketName,
    string GdFilePath,
    string FileHash,
    DateTime FileCreatedDate,
    DateTime FileLastModifiedDate
);
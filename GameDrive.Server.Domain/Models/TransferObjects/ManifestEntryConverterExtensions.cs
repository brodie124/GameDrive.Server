namespace GameDrive.Server.Domain.Models.TransferObjects;

public static class ManifestEntryConverterExtensions
{
    public static ManifestEntryDto ToDto(this ManifestEntry manifestEntry)
    {
        return new ManifestEntryDto()
        {
            Guid = manifestEntry.Guid,
            IsDeleted = manifestEntry.IsDeleted,
            RelativePath = manifestEntry.RelativePath,
            FileHash = manifestEntry.FileHash,
            FileSize = manifestEntry.FileSize,
            ClientPreviousLastModifiedDate = manifestEntry.ClientPreviousLastModifiedDate,
            LastModifiedDate = manifestEntry.LastModifiedDate,
            CreatedDate = manifestEntry.CreatedDate
        };
    }
    
    public static ManifestEntry ToManifestEntry(this ManifestEntryDto manifestEntryDto)
    {
        return new ManifestEntry()
        {
            Guid = manifestEntryDto.Guid,
            IsDeleted = manifestEntryDto.IsDeleted,
            RelativePath = manifestEntryDto.RelativePath,
            FileHash = manifestEntryDto.FileHash,
            FileSize = manifestEntryDto.FileSize,
            ClientPreviousLastModifiedDate = manifestEntryDto.ClientPreviousLastModifiedDate,
            LastModifiedDate = manifestEntryDto.LastModifiedDate,
            CreatedDate = manifestEntryDto.CreatedDate
        };
    }
}
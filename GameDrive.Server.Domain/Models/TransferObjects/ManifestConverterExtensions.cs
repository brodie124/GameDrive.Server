namespace GameDrive.Server.Domain.Models.TransferObjects;

public static class ManifestConverterExtensions
{
    public static GameProfileManifestDto ToDto(this GameProfileManifest manifest)
    {
        return new GameProfileManifestDto()
        {
            GameProfileId = manifest.GameProfileId,
            Entries = manifest.Entries.Select(x => x.ToDto()).ToList()
        };
    }
    
    public static GameProfileManifest ToGameProfileManifest(this GameProfileManifestDto manifest)
    {
        return new GameProfileManifest()
        {
            GameProfileId = manifest.GameProfileId,
            Entries = manifest.Entries.Select(x => x.ToManifestEntry()).ToList()
        };
    }
}
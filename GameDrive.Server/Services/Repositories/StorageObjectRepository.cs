using GameDrive.Server.Database;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Storage;

namespace GameDrive.Server.Services.Repositories;

public class StorageObjectRepository : GenericRepository<StorageObject>
{
    public StorageObjectRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
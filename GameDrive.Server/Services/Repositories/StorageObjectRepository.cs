using GameDrive.Server.Domain.Database;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Repositories;

public class StorageObjectRepository : GenericRepository<StorageObject>, IStorageObjectRepository
{
    public StorageObjectRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
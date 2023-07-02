using GameDrive.Server.Database;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Repositories;

public class BucketRepository : GenericRepository<Bucket>, IBucketRepository
{
    public BucketRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
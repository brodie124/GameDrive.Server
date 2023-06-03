using GameDrive.Server.Database;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Repositories;

public class GameProfileRepository : GenericRepository<Bucket>
{
    public GameProfileRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
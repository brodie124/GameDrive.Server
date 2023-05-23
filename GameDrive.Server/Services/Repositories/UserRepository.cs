using GameDrive.Server.Database;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Repositories;

public class UserRepository : GenericRepository<User>
{
    public UserRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
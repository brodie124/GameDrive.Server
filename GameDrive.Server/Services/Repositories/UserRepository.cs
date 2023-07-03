using GameDrive.Server.Domain.Database;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(GameDriveDbContext dbContext) : base(dbContext)
    {
    }
}
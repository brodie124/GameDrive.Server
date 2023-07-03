using GameDrive.Server.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GameDrive.Server.Domain.Database;

public class GameDriveDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<StorageObject> StorageObjects { get; set; } = default!;
    
    public GameDriveDbContext(DbContextOptions options) : base(options)
    {
    }
}
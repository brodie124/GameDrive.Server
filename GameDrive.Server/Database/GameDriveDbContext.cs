using GameDrive.Server.Domain.Models;
using GameDrive.Server.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Database;

public class GameDriveDbContext : DbContext
{
    private readonly DatabaseOptions? _databaseConfig;

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<StorageObject> StorageObjects { get; set; } = default!;

    public GameDriveDbContext(IOptions<DatabaseOptions> databaseOptions)
    {
        _databaseConfig = databaseOptions.Value;
    }

    protected GameDriveDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_databaseConfig is null)
            return;

        var connectionString = _databaseConfig.ConnectionString;
        var serverVersion = MySqlServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
}
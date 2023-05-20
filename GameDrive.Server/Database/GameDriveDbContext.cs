using GameDrive.Server.Domain.Models;
using GameDrive.Server.OptionsModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Database;

public class GameDriveDbContext : DbContext
{
    private readonly DatabaseOptions _databaseConfig;

    public DbSet<User> Users { get; set; } = default!;

    public GameDriveDbContext(IOptions<DatabaseOptions> databaseOptions)
    {
        _databaseConfig = databaseOptions.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _databaseConfig.ConnectionString;
        var serverVersion = MySqlServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
}
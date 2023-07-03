using GameDrive.Server.Domain.Database;
using GameDrive.Server.Models.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Tests;

public class SqliteInMemoryDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<GameDriveDbContext> _contextOptions;

    public SqliteInMemoryDatabase()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<GameDriveDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    public GameDriveDbContext CreateContext() => new GameDriveTestDbContext(_contextOptions);
    public void Dispose() => _connection.Dispose();
    
}

public class GameDriveTestDbContext : GameDriveDbContext
{
    public GameDriveTestDbContext(DbContextOptions options) : base(options)
    {
    }
}
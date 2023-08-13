using GameDrive.Server.Domain.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameDrive.Server.Tests.TestBed;

public class SqliteInMemoryDatabase
{
    private readonly SqliteConnection _connection;
    private readonly string _connectionString;

    private SqliteInMemoryDatabase()
    {
        var randomId = Guid.NewGuid().ToString().Replace("-", "");
        _connectionString = $"Data Source=GameDriveTestDb-{randomId};Mode=Memory;Cache=Shared";
        _connection = new SqliteConnection(_connectionString);
    }
    
    public void ResetAndRestart()
    {
        _connection.Close();
        _connection.Open();
    }

    public void RegisterTestDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameDriveDbContext>));
        if (descriptor != null)
            services.Remove(descriptor);
        
        services.AddDbContext<GameDriveDbContext>(options =>
        {
            options.UseSqlite(_connectionString, x => x.MigrationsAssembly(DatabaseProvider.Sqlite.Assembly));    
        });
    }
    
    public static SqliteInMemoryDatabase GetInstance()
    {
        return new SqliteInMemoryDatabase();
    }
}

public class GameDriveTestDbContext : GameDriveDbContext
{
    public GameDriveTestDbContext(DbContextOptions options) : base(options)
    {
    }
}
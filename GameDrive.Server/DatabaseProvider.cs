namespace GameDrive.Server;

public record DatabaseProvider(string Name, string Assembly) 
{
    public static readonly DatabaseProvider Sqlite = new (nameof(Sqlite), typeof(Migrations.SQLite.Marker).Assembly.GetName().Name!);
    public static readonly DatabaseProvider Mysql = new (nameof(Mysql), typeof(Migrations.MySQL.Marker).Assembly.GetName().Name!);
}
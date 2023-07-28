namespace GameDrive.Server.Tasks;

public interface IRepeatingTask
{
    TimeSpan Interval { get; }

    Task ExecuteAsync();
}
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Tasks.Startup;

public class UpdateGameProfilesTask : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly GameProfile[] _gameProfiles = {
        new GameProfile
        {
            Name = "RimWorld",
            Version = 1,
            SearchableDirectories = new List<string>()
            {
                @"%APPDATA%\..\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Saves"
            },
            IncludePatterns = new List<string>()
            {
                @".*\.rws$"
            }
        }
    };

    public UpdateGameProfilesTask(IServiceScopeFactory  serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var gameProfileRepository = scope.ServiceProvider.GetRequiredService<GameProfileRepository>();
        
        foreach (var profile in _gameProfiles)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var currentProfile = (await gameProfileRepository
                .FindAsync(x => x.Name.Equals(profile.Name)))
                .FirstOrDefault();

            if (currentProfile is null)
            {
                // Profile doesn't exist. Create it and move on.
                await gameProfileRepository.AddAsync(profile);
                continue;
            }

            if (profile.Version <= currentProfile.Version)
            {
                // The update profile version has a lower number. Let's not touch it and move on.
                continue;
            }

            profile.Id = currentProfile.Id;
            await gameProfileRepository.UpdateAsync(profile);
        }

        await gameProfileRepository.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
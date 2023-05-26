using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class GameProfileController : ControllerBase
{
    private readonly GameProfileRepository _gameProfileRepository;

    public GameProfileController(GameProfileRepository gameProfileRepository)
    {
        _gameProfileRepository = gameProfileRepository;
    }

    [HttpGet("List")]
    public async Task<ApiResponse<IReadOnlyCollection<GameProfile>>> GetGameProfilesAsync()
    {
        return ApiResponse<IReadOnlyCollection<GameProfile>>.Success(await _gameProfileRepository.GetAllAsync());
    }

}
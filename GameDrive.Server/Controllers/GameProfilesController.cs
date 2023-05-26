using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class GameProfilesController : ControllerBase
{
    private readonly GameProfileRepository _gameProfileRepository;

    public GameProfilesController(GameProfileRepository gameProfileRepository)
    {
        _gameProfileRepository = gameProfileRepository;
    }

    [HttpGet]
    public async Task<ApiResponse<IReadOnlyCollection<GameProfile>>> GetGameProfilesAsync()
    {
        return ApiResponse<IReadOnlyCollection<GameProfile>>.Success(await _gameProfileRepository.GetAllAsync());
    }

}
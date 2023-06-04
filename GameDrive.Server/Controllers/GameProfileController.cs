using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class GameProfileController : ControllerBase
{
    private readonly BucketRepository _bucketRepository;

    public GameProfileController(BucketRepository bucketRepository)
    {
        _bucketRepository = bucketRepository;
    }

    [HttpGet("List")]
    public async Task<ApiResponse<IReadOnlyCollection<Bucket>>> GetGameProfilesAsync()
    {
        return ApiResponse<IReadOnlyCollection<Bucket>>.Success(await _bucketRepository.GetAllAsync());
    }

}
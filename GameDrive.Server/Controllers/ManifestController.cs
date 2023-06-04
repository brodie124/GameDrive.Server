using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Extensions;
using GameDrive.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ManifestController : ControllerBase
{
    private readonly ManifestService _manifestService;

    public ManifestController(ManifestService manifestService)
    {
        _manifestService = manifestService;
    }
    
    [HttpPost("Compare")]
    public async Task<ApiResponse<List<ManifestFileReport>>> CompareManifestAsync([FromBody] FileManifest fileManifest)
    {
        var jwtData = Request.GetJwtDataFromRequest();
        var report = await _manifestService.GenerateComparisonReport(jwtData.UserId, fileManifest);
        return report;
    }
}
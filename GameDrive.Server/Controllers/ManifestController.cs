using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Domain.Models.TransferObjects;
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
    public async Task<ApiResponse<CompareManifestResponse>> CompareManifestAsync([FromBody] CompareManifestRequest compareManifestRequest)
    {
        var jwtData = Request.GetJwtDataFromRequest();
        var manifest = compareManifestRequest.Manifest.ToGameProfileManifest();
        var report = await _manifestService.GenerateComparisonReport(jwtData.UserId, manifest);
        return report;
    }
}
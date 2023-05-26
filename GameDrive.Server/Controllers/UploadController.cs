using GameDrive.Server.Attributes;
using GameDrive.Server.Extensions;
using GameDrive.Server.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace GameDrive.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly StorageService _storageService;

    public UploadController(StorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> UploadFileAsync(
        [FromQuery] int gameProfileId, 
        [FromQuery] string fileNameWithExtension,
        [FromQuery] string fileHash,
        CancellationToken cancellationToken = default
    )
    {
        var jwtData = Request.GetJwtDataFromRequest();
        var boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;
        
        var reader = new MultipartReader(boundary, Request.Body);
        var fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
        var extension = Path.GetExtension(fileNameWithExtension);
        var result = await _storageService.UploadFileAsync(new SaveStorageObjectRequest(
            OwnerId: jwtData.UserId,
            GameProfileId: gameProfileId,
            FileName: fileName,
            FileExtension: extension,
            FileHash: fileHash,
            MultipartReader: reader
        ), cancellationToken);
        
        if (result is null)
            return UnprocessableEntity();

        return Ok();
    }
}
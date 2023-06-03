using GameDrive.Server.Attributes;
using GameDrive.Server.Domain.Models.Responses;
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
    public async Task<ApiResponse<bool>> UploadFileAsync(
        [FromQuery] string bucketId, 
        [FromQuery] string fileNameWithExtension,
        [FromQuery] string fileHash,
        [FromQuery] DateTime fileCreatedDate,
        [FromQuery] DateTime fileLastModifiedDate,
        CancellationToken cancellationToken = default
    )
    {
        var jwtData = Request.GetJwtDataFromRequest();
        var boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;
        
        var reader = new MultipartReader(boundary, Request.Body);
        var fileName = Path.GetFileName(fileNameWithExtension); // File name with extension
        var result = await _storageService.UploadFileAsync(new SaveStorageObjectRequest(
            OwnerId: jwtData.UserId,
            BucketId: bucketId,
            FileName: fileName,
            FileHash: fileHash,
            FileCreatedDate: fileCreatedDate,
            FileLastModifiedDate: fileLastModifiedDate,
        MultipartReader: reader
        ), cancellationToken);

        if (result is null)
            return false;

        return true;
    }
}
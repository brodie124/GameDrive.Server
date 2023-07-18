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

    // FIXME: lower the number of parameters for this function
    [HttpPost]
    [DisableFormValueModelBinding]
    public async Task<ApiResponse<bool>> UploadFileAsync(
        [FromQuery] string bucketId, 
        [FromQuery] string bucketName,
        [FromQuery] string gdFilePath,
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
        var result = await _storageService.UploadFileAsync(new SaveStorageObjectRequest(
            OwnerId: jwtData.UserId,
            BucketId: bucketId,
            BucketName: bucketName,
            GdFilePath: gdFilePath,
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
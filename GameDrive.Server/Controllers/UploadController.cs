using GameDrive.Server.Attributes;
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
    public async Task<IActionResult> UploadFileAsync(CancellationToken cancellationToken = default)
    {
        var boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;
        var reader = new MultipartReader(boundary, Request.Body);

        var result = await _storageService.UploadFileAsync("testfile.json", reader, cancellationToken);
        if (result is null)
            return UnprocessableEntity();

        return Ok();
    }
}
using System.Net.Mime;
using GameDrive.Server.Services.Repositories;
using GameDrive.Server.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IStorageObjectRepository _storageObjectRepository;
    private readonly IStorageProvider _storageProvider;

    public DownloadController(
        IStorageObjectRepository storageObjectRepository,
        IStorageProvider storageProvider
    )
    {
        _storageObjectRepository = storageObjectRepository;
        _storageProvider = storageProvider;
    }

    [HttpGet("{storageObjectId}")]
    public async Task<ActionResult> DownloadStorageObjectAsync(Guid storageObjectId)
    {
        var storageObject = (await _storageObjectRepository
                .FindAsync(x => x.Id == storageObjectId))
            .FirstOrDefault();

        if (storageObject is null)
            return NotFound();

        var downloadLink = await _storageProvider.GenerateDownloadLinkAsync(storageObject);
        if (!downloadLink.Success || string.IsNullOrWhiteSpace(downloadLink.DownloadUrl))
            return UnprocessableEntity();
        
        return Redirect(downloadLink.DownloadUrl);
    }
    
    [HttpGet("Local/{storageObjectId}")]
    public async Task<ActionResult> DownloadLocalStorageObjectAsync(Guid storageObjectId)
    {
        var storageObject = (await _storageObjectRepository
                .FindAsync(x => x.Id == storageObjectId))
            .FirstOrDefault();

        if (storageObject is null)
            return NotFound();

        // FIXME: this path needs to be validated (i.e. jailed)
        if (!System.IO.File.Exists(storageObject.GameDrivePath))
            return NotFound();

        var stream = System.IO.File.OpenRead(storageObject.GameDrivePath);
        return new FileStreamResult(stream, MediaTypeNames.Application.Octet)
        {
            FileDownloadName = storageObject.ClientRelativePath
        };
    }
}
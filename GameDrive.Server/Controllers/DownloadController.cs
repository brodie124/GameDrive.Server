using System.Net.Mime;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IStorageObjectRepository _storageObjectRepository;

    public DownloadController(IStorageObjectRepository storageObjectRepository)
    {
        _storageObjectRepository = storageObjectRepository;
    }

    [HttpGet("{storageObjectId}")]
    public async Task<ActionResult> DownloadStorageObjectAsync(Guid storageObjectId)
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
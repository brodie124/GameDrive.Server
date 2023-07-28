using System.Text.Json;
using GameDrive.Server.Attributes;
using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Extensions;
using GameDrive.Server.Services.Storage;
using GameDrive.Server.Utilities;
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
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly StorageService _storageService;
    private readonly TemporaryStorageProvider _temporaryStorageProvider;

    public UploadController(
        IWebHostEnvironment hostEnvironment,
        StorageService storageService,
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        _hostEnvironment = hostEnvironment;
        _storageService = storageService;
        _temporaryStorageProvider = temporaryStorageProvider;
    }

    // FIXME: lower the number of parameters for this function
    [HttpPost]
    [DisableFormValueModelBinding]
    public async Task<ApiResponse<bool>> UploadFileAsync(
        CancellationToken cancellationToken = default
    )
    {
        if(_hostEnvironment.IsProduction())
            return ApiResponse<bool>.Failure("This feature is currently under development.");
        
        var jwtData = Request.GetJwtDataFromRequest();
        var boundary = HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
        ).Value;

        var reader = new MultipartReader(boundary, Request.Body);
        
        var temporaryFiles = new Dictionary<string, Guid>();
        var uploadFileRequests = new List<UploadFileRequest>();
        

        while(true)
        {
            var section = await reader.ReadNextSectionAsync(cancellationToken);
            if (section is null)
                break;

            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                continue;

            if (contentDisposition.Name.Value?.ToLower() == "gd-metadata")
            {
                var uploadMetadata = await JsonSerializer.DeserializeAsync<UploadFileRequest>(
                    utf8Json: section.Body, 
                    cancellationToken: cancellationToken
                );

                ArgumentNullException.ThrowIfNull(uploadMetadata);
                uploadFileRequests.Add(uploadMetadata);
                continue;
            }

            if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                continue;

            if (string.IsNullOrWhiteSpace(contentDisposition.Name.Value))
                continue;
            
            var saveTempResult = await _temporaryStorageProvider.SaveFileAsync(section.Body);
            temporaryFiles.Add(contentDisposition.Name.Value, saveTempResult.Key);
        }

        if (uploadFileRequests.Count == 0)
            return ApiResponse<bool>.Failure("No upload file request was specified in the gd-metadata field.");
        
        if (uploadFileRequests.Count != temporaryFiles.Count)
            return ApiResponse<bool>.Failure("A mismatched number of gd-metadata and gd-files entries were present");

        if (temporaryFiles.Count == 0)
            return ApiResponse<bool>.Failure("No files were successfully uploaded");
        
        var saveRequests = new List<SaveStorageObjectRequest>();
        foreach (var (multiPartName, temporaryFileKey) in temporaryFiles)
        {
            var uploadFileRequest = uploadFileRequests.First(x => x.MultiPartName == multiPartName);
            var saveStorageObjectRequest = new SaveStorageObjectRequest(
                OwnerId: jwtData.UserId,
                BucketId: uploadFileRequest.BucketId,
                BucketName: uploadFileRequest.BucketName,
                GdFilePath: uploadFileRequest.GdFilePath,
                FileHash: uploadFileRequest.FileHash,
                FileCreatedDate: uploadFileRequest.FileCreatedDate,
                FileLastModifiedDate: uploadFileRequest.FileLastModifiedDate,
                TemporaryFileKey: temporaryFileKey
            );
            
            saveRequests.Add(saveStorageObjectRequest);
        }

        await _storageService.SaveFilesAsync(saveRequests, cancellationToken);
        return true;
    }
}
using Microsoft.AspNetCore.Http;

namespace GameDrive.Server.Domain.Models.Requests;

public record UploadFileRequest(
    IFormFile File
);
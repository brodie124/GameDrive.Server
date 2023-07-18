using System.Reflection;
using GameDrive.Server.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{

    [HttpGet]
    public ApiResponse<HealthCheckResponse> GetHealthCheck()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        return new HealthCheckResponse(true, version);
    }
    
}
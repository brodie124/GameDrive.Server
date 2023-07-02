using GameDrive.Server.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{

    [HttpGet]
    public ApiResponse<string> GetHealthCheck()
    {
        return "Healthy";
    }
    
}
using GameDrive.Server.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Models;

public class GdsResponse<T> : ApiResponse<T>
{
    public static implicit operator ActionResult(GdsResponse<T> gdsResponse) => new ObjectResult(gdsResponse);
    protected GdsResponse()
    {
        
    }
}

public class GdsResponse : GdsResponse<object>
{
    // public static implicit operator GdsResponse<T>(GdsResponse<T> gdsResponse) => new ObjectResult(gdsResponse);
}
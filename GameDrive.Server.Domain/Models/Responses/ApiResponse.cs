namespace GameDrive.Server.Domain.Models.Responses;

public enum ApiResponseCode
{
    OK = 1,
    GenericError = 2,
    
    #region Authentication
    AuthInvalidCredentials = 3,
    AuthUsernameTaken = 4,
    #endregion
}

public class ApiResponse<T>
{
    public static implicit operator ApiResponse<T>(T obj) => ApiResponse<T>.Success(obj);

    public bool IsSuccess => ResponseCode == ApiResponseCode.OK;
    public ApiResponseCode ResponseCode { get; init; } = ApiResponseCode.OK;
    
    public Exception? InnerException { get; init; }
    public string? ErrorMessage { get; init; }
    
    public T? Data { get; init; }

    protected ApiResponse()
    {
        
    }

    public static ApiResponse<T> Success(T? resultObject)
    {
        return new ApiResponse<T>()
        {
            ResponseCode = ApiResponseCode.OK,
            Data = resultObject
        };
    }

    public static ApiResponse<T> Failure(string? message, ApiResponseCode responseCode = ApiResponseCode.GenericError)
    {
        return ApiResponse<T>.Failure(
            ex: null,
            message: message,
            responseCode: responseCode
        );
    }

    public static ApiResponse<T> Failure(Exception? ex, string? message, ApiResponseCode responseCode = ApiResponseCode.GenericError)
    {
        return new ApiResponse<T>()
        {
            ResponseCode = responseCode,
            InnerException = ex,
            ErrorMessage = message
        };
    }
}

// public class ApiResponse : ApiResponse<object>
// {
//     public static explicit operator ApiResponse(ApiResponse<object> apiResponse) => (ApiResponse) apiResponse;
// }
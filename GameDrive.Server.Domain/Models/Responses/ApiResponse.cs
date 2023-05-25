using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public bool IsSuccess => ResponseCode == ApiResponseCode.OK;
    
    [JsonPropertyName("responseCode")]
    public ApiResponseCode ResponseCode { get; set; } = ApiResponseCode.OK;
    
    [JsonPropertyName("innerException")]
    public Exception? InnerException { get; set; }
    
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
    
    [JsonPropertyName("data")]
    public T? Data { get; set; }

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
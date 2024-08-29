using Core.ErrorHandling;

namespace API.Errors
{
    public class ApiExceptionResponse(int statusCode, string? message = null, string? errors = null) : ApiResponse(statusCode, message)
    {
        public string? Errors { get; set; } = errors;
    }
}
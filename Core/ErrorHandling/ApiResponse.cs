namespace Core.ErrorHandling;
public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; }

    public ApiResponse(int statusCode, string? title = null)
    {
        StatusCode = statusCode;
        Title = title ?? GetDefaultMessageForStatusCode(statusCode);
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            200 => "Success",
            400 => "A bad request, you have made!",
            401 => "Authorized, you are not!",
            404 => "Resourse was not found!",
            500 => "Server Error",
            _ => "Invalid request"
        };
    }
}
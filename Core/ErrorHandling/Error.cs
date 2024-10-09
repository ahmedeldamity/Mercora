namespace BlazorEcommerce.Domain.ErrorHandling;
public class Error
{
    public static readonly Error? None = new(200, string.Empty);

    public int StatusCode { get; set; }
    public string Title { get; set; }

    public Error(int statusCode, string? title = null)
    {
        StatusCode = statusCode;
        Title = title ?? GetDefaultMessageForStatusCode(statusCode);
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "A bad request, you have made!",
            401 => "Authorized, you are not!",
            404 => "Resource was not found!",
            500 => "Server Error",
            _ => "Invalid request"
        };
    }
}
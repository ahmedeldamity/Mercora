namespace BlazorEcommerce.Shared.Response;
public class ErrorResponse
{
	public int StatusCode { get; set; }
	public string Message { get; set; } = string.Empty;
}
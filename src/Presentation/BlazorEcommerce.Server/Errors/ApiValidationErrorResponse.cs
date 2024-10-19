namespace BlazorEcommerce.Server.Errors;
public class ApiValidationErrorResponse() : Error(400)
{
    public IEnumerable<string> Errors { get; set; } = [];
}
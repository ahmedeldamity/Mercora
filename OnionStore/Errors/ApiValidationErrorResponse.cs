using Core.ErrorHandling;

namespace API.Errors;
public class ApiValidationErrorResponse: Error
{
	public ApiValidationErrorResponse() : base(400) { }
	public IEnumerable<string> Errors { get; set; } = [];
}
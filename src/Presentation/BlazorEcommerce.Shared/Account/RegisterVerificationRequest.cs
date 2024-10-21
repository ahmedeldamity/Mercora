using System.ComponentModel.DataAnnotations;

namespace BlazorEcommerce.Shared.Account;
public class RegisterVerificationRequest
{
	[Required(ErrorMessage = "Name is required")]
	[StringLength(100, ErrorMessage = "Name must be at most 100 characters")]
	public string DisplayName { get; set; } = string.Empty;

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Invalid Email Address")]
	[StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
	public string Email { get; set; } = string.Empty;
}
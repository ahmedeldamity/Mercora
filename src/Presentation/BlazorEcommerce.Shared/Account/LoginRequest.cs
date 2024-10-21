using System.ComponentModel.DataAnnotations;

namespace BlazorEcommerce.Shared.Account;
public class LoginRequest
{
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Invalid Email Address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required")]
	[DataType(DataType.Password)]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
	[MaxLength(100, ErrorMessage = "Password must be at most 100 characters")]
	[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,100}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
    public string Password { get; set; } = string.Empty;

	[Required(ErrorMessage = "Confirm Password is required")]
	[DataType(DataType.Password)]
	[Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
	public string ConfirmPassword { get; set; } = string.Empty;
}
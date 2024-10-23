using System.ComponentModel.DataAnnotations;

namespace BlazorEcommerce.Shared.Account;
public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    //[RegularExpression(@"^\d{6}$", ErrorMessage = "Code must contain 6 digits")]
    public string Code { get; set; } = string.Empty;
}
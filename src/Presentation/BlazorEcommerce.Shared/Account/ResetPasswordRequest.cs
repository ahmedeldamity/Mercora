using System.ComponentModel.DataAnnotations;

namespace BlazorEcommerce.Shared.Account;
public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [StringLength(100, ErrorMessage = "Email must be at most 100 characters")]
    public string Email { get; set; } = string.Empty;
}
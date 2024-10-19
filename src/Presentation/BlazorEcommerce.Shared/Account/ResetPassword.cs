namespace BlazorEcommerce.Shared.Account;
public class ResetPassword
{
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
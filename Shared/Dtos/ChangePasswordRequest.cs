namespace Shared.Dtos;
public class ChangePasswordRequest
{
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
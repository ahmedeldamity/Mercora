namespace Shared.Dtos;
public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
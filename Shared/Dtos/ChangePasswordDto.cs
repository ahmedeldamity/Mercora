namespace Shared.Dtos;
public class ChangePasswordDto
{
    public string email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
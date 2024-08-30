namespace Core.Dtos;
public record ChangePasswordRequest
{
    public string NewPassword { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
namespace Shared.Dtos;
public class CodeVerificationRequest
{
    public string Email { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
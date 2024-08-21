namespace Shared.Dtos;
public class CodeVerificationDto
{
    public string Email { get; set; } = null!;
    public string VerificationCode { get; set; } = null!;
}
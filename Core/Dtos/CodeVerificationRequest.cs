namespace Core.Dtos;
public record CodeVerificationRequest
{
    public string VerificationCode { get; set; } = null!;
}
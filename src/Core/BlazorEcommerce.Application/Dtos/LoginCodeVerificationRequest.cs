namespace BlazorEcommerce.Application.Dtos;
public record LoginCodeVerificationRequest(string Email, string VerificationCode);
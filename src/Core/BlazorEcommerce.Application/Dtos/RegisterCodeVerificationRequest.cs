namespace BlazorEcommerce.Application.Dtos;
public record RegisterCodeVerificationRequest(string DisplayName, string Email, string VerificationCode);
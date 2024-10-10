namespace BlazorEcommerce.Application.Dtos;
public record CodeVerificationRequest(
    string DisplayName,
    string Email,
    string VerificationCode
);
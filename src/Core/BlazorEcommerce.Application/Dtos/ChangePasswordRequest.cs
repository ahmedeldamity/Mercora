namespace BlazorEcommerce.Application.Dtos;
public record ChangePasswordRequest(
    string NewPassword,
    string VerificationCode
);
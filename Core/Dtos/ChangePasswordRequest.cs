namespace Core.Dtos;
public record ChangePasswordRequest(
    string NewPassword,
    string VerificationCode
);
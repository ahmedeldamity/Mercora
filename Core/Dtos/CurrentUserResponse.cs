namespace Core.Dtos;
public record CurrentUserResponse(
    string DisplayName,
    string Email,
    string Token
);
namespace BlazorEcommerce.Application.Dtos;
public record CurrentUserResponse(
    string DisplayName,
    string Email,
    string Token
);
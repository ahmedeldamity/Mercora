namespace BlazorEcommerce.Shared.Account;
public record AppUserResponse(
    string DisplayName,
    string Email,
    string Token,
    DateTime RefreshTokenExpireAt
);
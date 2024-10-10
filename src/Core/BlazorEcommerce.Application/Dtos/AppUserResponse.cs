namespace BlazorEcommerce.Application.Dtos;
public record AppUserResponse(
    string DisplayName,
    string Email,
    string Token,
    DateTime RefreshTokenExpireAt
);

public record AppUserResponseV20(
    string DisplayName,
    string Email,
    string Token,
    string RefreshTokenExpireAt
);

public record AppUserResponseV21(

    string FirstName,
    string LastName,
    string Email,
    string Token,
    string RefreshTokenExpireAt
);
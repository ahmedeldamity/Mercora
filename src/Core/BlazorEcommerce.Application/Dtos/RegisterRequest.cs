namespace BlazorEcommerce.Application.Dtos;
public record RegisterRequest(
    string DisplayName,
    string Email
);

public record RegisterRequestV2(
    string FirstName,
    string LastName,
    string Email,
    string Password
);
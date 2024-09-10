namespace Core.Dtos;
public record RegisterRequest(
    string DisplayName,
    string Email,
    string PhoneNumber,
    string Password
);

public record RegisterRequestV2(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password
);
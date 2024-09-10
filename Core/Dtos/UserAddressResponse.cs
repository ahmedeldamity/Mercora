namespace Core.Dtos;
public record UserAddressResponse(
    string FirstName,
    string LastName,
    string Street,
    string City,
    string Country
);
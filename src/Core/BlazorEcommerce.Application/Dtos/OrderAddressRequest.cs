namespace BlazorEcommerce.Application.Dtos;
public record OrderAddressRequest(
    string FirstName,
    string LastName,
    string Street,
    string City,
    string Country
);
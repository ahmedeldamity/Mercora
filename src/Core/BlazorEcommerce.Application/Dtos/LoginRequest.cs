namespace BlazorEcommerce.Application.Dtos;
public record LoginRequest(
	string Email,
    string Password
);
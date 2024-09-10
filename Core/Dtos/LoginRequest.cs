namespace Core.Dtos;
public record LoginRequest(
	string Email,
    string Password
);
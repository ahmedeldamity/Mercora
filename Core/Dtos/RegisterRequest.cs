namespace Core.Dtos;
public record RegisterRequest
{
	public string DisplayName { get; set; } = null!;
	public string Email { get; set; } = null!;
	public string PhoneNumber { get; set; } = null!;
	public string Password { get; set; } = null!;
}
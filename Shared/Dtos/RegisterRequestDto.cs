using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;
public class RegisterRequestDto
{
	public string DisplayName { get; set; } = null!;

	[EmailAddress]
	public string Email { get; set; } = null!;

	[Phone]
	public string PhoneNumber { get; set; } = null!;

	public string Password { get; set; } = null!;
}
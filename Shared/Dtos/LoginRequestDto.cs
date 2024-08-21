using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;
public class LoginRequestDto
{
	[EmailAddress]
	public string Email { get; set; } = null!;

	public string Password { get; set; } = null!;
}
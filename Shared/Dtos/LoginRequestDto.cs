using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
	public class LoginRequestDto
	{
		[EmailAddress]
		public string Email { get; set; }

		public string Password { get; set; }
	}
}

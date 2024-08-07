using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
	public class RegisterRequestDto
	{
		public string DisplayName { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		public string PhoneNumber { get; set; }

		public string Password { get; set; }
	}
}

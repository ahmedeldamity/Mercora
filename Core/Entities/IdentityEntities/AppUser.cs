using Microsoft.AspNetCore.Identity;

namespace Core.Entities.IdentityEntities
{
	public class AppUser: IdentityUser
	{
		public string DisplayName { get; set; }
		public UserAddress Address { get; set; }
	}
}

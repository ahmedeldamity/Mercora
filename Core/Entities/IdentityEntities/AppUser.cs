using Microsoft.AspNetCore.Identity;

namespace Core.Entities.IdentityEntities;
public class AppUser: IdentityUser
{
	public string DisplayName { get; set; } = null!;
	public UserAddress Address { get; set; } = null!;
	public ICollection<RefreshToken>? RefreshTokens { get; set; } = [];
}
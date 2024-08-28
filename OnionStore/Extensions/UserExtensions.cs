using System.Security.Claims;

namespace Freelancers.Api.Extensions;

public static class UserExtensions
{
	public static string? GetUserId(this ClaimsPrincipal claims) =>
		claims.FindFirstValue(ClaimTypes.NameIdentifier);
}

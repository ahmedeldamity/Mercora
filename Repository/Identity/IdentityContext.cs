using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository.Identity
{
	public class IdentityContext: IdentityDbContext<IdentityUser>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }
	}
}

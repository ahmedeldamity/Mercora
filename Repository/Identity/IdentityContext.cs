using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository.Identity
{
	public class IdentityContext: IdentityDbContext<AppUser> // we inherit from IdentityDbContext not DbContext to get 7 dbcontext
	{
		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

		// In this method we override OnModelCreating which exist in base class
		// so we need to call it
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<UserAddress>()
				.ToTable("Addresses");
		}
	}
}

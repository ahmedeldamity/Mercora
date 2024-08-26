using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repository.Identity;
public class IdentityContext: IdentityDbContext<AppUser> // we inherit from IdentityDbContext not DbContext to get 7 dbcontext
{
	public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) 
	{
		try
		{
            var dataCreater = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;

			if(dataCreater != null)
			{
				if (!dataCreater.CanConnect())
					dataCreater.Create();

				if (!dataCreater.HasTables())
					dataCreater.CreateTables();
            }
        }
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	// In this method we override OnModelCreating which exist in base class
	// so we need to call it
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<UserAddress>()
			.ToTable("Addresses");
	}

    public DbSet<IdentityCode> IdentityCodes { get; set; }
}
using Core.Entities;
using Core.Entities.IdentityEntities;
using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Repository.Store;
public class StoreContext(DbContextOptions<StoreContext> options) : IdentityDbContext<AppUser>(options) // we inherit from IdentityDbContext not DbContext to get 7 dbcontext
{

    // In this method we override OnModelCreating which exist in base class
    // so we need to call it
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //modelBuilder.ApplyConfiguration(new ProductConfigurations());
        //modelBuilder.ApplyConfiguration(new ProductBrandConfigurations());
        //modelBuilder.ApplyConfiguration(new ProductCategoryConfigurations());

        // -- New Way
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<IdentityCode> IdentityCodes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBrand> Brands { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderDeliveryMethod> OrderDeliveryMethods { get; set; }
}
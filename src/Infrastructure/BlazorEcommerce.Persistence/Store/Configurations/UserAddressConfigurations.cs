namespace BlazorEcommerce.Persistence.Store.Configurations;
public class UserAddressConfigurations: IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("Addresses");
    }
}
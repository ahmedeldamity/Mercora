namespace BlazorEcommerce.Persistence.Store.Configurations;
public class IdentityCodeConfigurations: IEntityTypeConfiguration<IdentityCode>
{
    public void Configure(EntityTypeBuilder<IdentityCode> builder)
    {
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(200);
    }
}
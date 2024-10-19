namespace BlazorEcommerce.Persistence.Store.Configurations;
public class ProductBrandConfigurations: IEntityTypeConfiguration<ProductBrand>
{
    public void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.Property(p => p.ImageCover)
            .HasMaxLength(300);
    }
}
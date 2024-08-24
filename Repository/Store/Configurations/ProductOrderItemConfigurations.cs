using Core.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;
public class ProductOrderItemConfigurations: IEntityTypeConfiguration<ProductOrderItem>
{
    public void Configure(EntityTypeBuilder<ProductOrderItem> builder)
    {
        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ProductImageCover)
            .IsRequired()
            .HasMaxLength(100);
    }
}
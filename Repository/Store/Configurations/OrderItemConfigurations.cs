using BlazorEcommerce.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorEcommerce.Persistence.Store.Configurations;
public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(orderItem => orderItem.Product, product =>
        {
            product.WithOwner();

            product.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(100);

            product.Property(p => p.ProductImageCover)
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.Property(p => p.Price)
            .HasColumnType("decimal(6,2)");

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(9,2)");
    }
}
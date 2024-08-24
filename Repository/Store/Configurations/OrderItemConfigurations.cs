using Core.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;
public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(orderItem => orderItem.Product, Product =>
        {
            Product.WithOwner();

            Product.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(100);

            Product.Property(p => p.ProductImageCover)
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.Property(p => p.Price)
            .HasColumnType("decimal(6,2)");

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(9,2)");
    }
}
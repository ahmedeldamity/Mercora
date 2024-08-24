using Core.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;
public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(orderItem => orderItem.Product, Product => Product.WithOwner());

        builder.Property(p => p.Price)
            .HasColumnType("decimal(6,2)");

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(8,2)");
    }
}
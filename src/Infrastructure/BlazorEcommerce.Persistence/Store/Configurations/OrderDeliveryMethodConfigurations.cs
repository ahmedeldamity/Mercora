using BlazorEcommerce.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorEcommerce.Persistence.Store.Configurations;
public class OrderDeliveryMethodConfigurations : IEntityTypeConfiguration<OrderDeliveryMethod>
{
    public void Configure(EntityTypeBuilder<OrderDeliveryMethod> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);


        builder.Property(p => p.DeliveryTime)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Cost)
            .HasColumnType("decimal(6,2)");
    }
}
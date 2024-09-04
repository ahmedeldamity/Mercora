using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities.OrderEntities;

namespace Repository.Store.Configurations;
public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(o => o.ShippingAddress, SAddress => SAddress.WithOwner());

        builder.Property(p => p.BuyerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Status)
            .HasConversion(
                OStatus => OStatus.ToString(),
                OStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus)
            );
        #region Explaination
        // here we store in OrderStatus in database as string 
        // and when we bring it from batabase will bring it as Enum of type OrderStatus
        // so if (property which recieve OrderState from batabase) was int => then this property will be number (0 | 1 | 2 ..)
        // and if was string => then this property will be (pending | Payment Succeded | Payment Failed)
        #endregion

        builder.Property(p => p.SubTotal)
            .HasColumnType("decimal(9,2)");

        builder.HasOne(O => O.DeliveryMethod)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
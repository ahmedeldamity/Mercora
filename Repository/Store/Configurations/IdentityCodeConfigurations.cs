using BlazorEcommerce.Domain.Entities.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorEcommerce.Persistence.Store.Configurations;
public class IdentityCodeConfigurations: IEntityTypeConfiguration<IdentityCode>
{
    public void Configure(EntityTypeBuilder<IdentityCode> builder)
    {
        builder.Property(p => p.AppUserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(6);
    }
}
using Core.Entities.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;
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
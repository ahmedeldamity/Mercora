using Core.Entities.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
    }
}
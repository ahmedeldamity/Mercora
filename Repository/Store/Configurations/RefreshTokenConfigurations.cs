using Core.Entities.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Store.Configurations;
public class RefreshTokenConfigurations: IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(p => p.Token)
            .IsRequired()
            .HasMaxLength(100);
    }
}
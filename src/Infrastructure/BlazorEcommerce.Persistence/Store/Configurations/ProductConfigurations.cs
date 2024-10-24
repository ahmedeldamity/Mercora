﻿namespace BlazorEcommerce.Persistence.Store.Configurations;
public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ImageCover)
            .HasMaxLength(300);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.RatingsAverage)
            .HasColumnType("decimal(18,2)");

        //builder.HasOne(p => p.Brand)
        //    .WithMany()
        //    .HasForeignKey(fk => fk.BrandId);

        //builder.HasOne(p => p.Category)
        //   .WithMany()
        //   .HasForeignKey(fk => fk.CategoryId);
    }
}
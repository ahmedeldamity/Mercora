﻿namespace BlazorEcommerce.Persistence.Store.Configurations;
public class ProductCategoryConfigurations: IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(100);
    }
}
using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using Microsoft.Extensions.Configuration;

namespace BlazorEcommerce.Application.MappingProfıles;
public class ProductBrandImageCoverResolver(IConfiguration configuration) : IValueResolver<ProductBrand, ProductBrandResponse, string>
{
    public string Resolve(ProductBrand source, ProductBrandResponse destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.ImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.ImageCover}" : string.Empty;
    }
}
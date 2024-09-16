using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductEntities;

namespace API.Helpers;
public class ProductBrandImageCoverResolver(IConfiguration configuration) : IValueResolver<ProductBrand, ProductBrandResponse, string>
{
    public string Resolve(ProductBrand source, ProductBrandResponse destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.ImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.ImageCover}" : string.Empty;
    }
}
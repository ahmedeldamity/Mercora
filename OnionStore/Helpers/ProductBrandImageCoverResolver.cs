using AutoMapper;
using Core.Entities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductBrandImageCoverResolver(IConfiguration _configuration) : IValueResolver<ProductBrand, ProductBrandResponse, string>
{
    public string Resolve(ProductBrand source, ProductBrandResponse destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.ImageCover))
        {
            return $"{_configuration["ApiBaseUrl"]}/{source.ImageCover}";
        }
        return string.Empty;
    }
}
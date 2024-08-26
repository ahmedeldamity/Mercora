using AutoMapper;
using Core.Entities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductBrandImageCoverResolver : IValueResolver<ProductBrand, ProductBrandResponse, string>
{
    private readonly IConfiguration _configuration;

    public ProductBrandImageCoverResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string Resolve(ProductBrand source, ProductBrandResponse destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.ImageCover))
        {
            return $"{_configuration["ApiBaseUrl"]}/{source.ImageCover}";
        }
        return string.Empty;
    }
}
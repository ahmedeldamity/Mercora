using AutoMapper;
using Core.Entities.Product_Entities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductImageCoverResolver : IValueResolver<Product, ProductToReturnDto, string>
{
    private readonly IConfiguration _configuration;

    public ProductImageCoverResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.ImageCover))
        {
            return $"{_configuration["ApiBaseUrl"]}/{source.ImageCover}";
        }
        return string.Empty;
    }
}
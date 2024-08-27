using AutoMapper;
using Core.Entities.Product_Entities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductImageCoverResolver(IConfiguration _configuration) : IValueResolver<Product, ProductResponse, string>
{
    public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.ImageCover))
        {
            return $"{_configuration["ApiBaseUrl"]}/{source.ImageCover}";
        }
        return string.Empty;
    }
}
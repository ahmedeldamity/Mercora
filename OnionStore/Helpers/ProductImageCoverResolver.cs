using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductEntities;

namespace API.Helpers;
public class ProductImageCoverResolver(IConfiguration configuration) : IValueResolver<Product, ProductResponse, string>
{
    public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.ImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.ImageCover}" : string.Empty;
    }
}
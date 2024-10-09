using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using Microsoft.Extensions.Configuration;

namespace BlazorEcommerce.Application.MappingProfıles;
public class ProductImageCoverResolver(IConfiguration configuration) : IValueResolver<Product, ProductResponse, string>
{
    public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.ImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.ImageCover}" : string.Empty;
    }
}
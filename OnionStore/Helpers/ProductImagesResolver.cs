using AutoMapper;
using Core.Entities.Product_Entities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductImagesResolver(IConfiguration _configuration) : IValueResolver<Product, ProductResponse, string[]>
{
    public string[] Resolve(Product source, ProductResponse destination, string[] destMember, ResolutionContext context)
    {
        string[] ImagesPath = new string[source.Images.Count()];
        if (source.Images is not null)
        {
            if (source.Images.Count() > 0)
                ImagesPath[0] = $"{_configuration["ApiBaseUrl"]}/{source.Images[0]}";
            if (source.Images.Count() > 1)
                ImagesPath[1] = $"{_configuration["ApiBaseUrl"]}/{source.Images[1]}";
            if (source.Images.Count() > 2)
                ImagesPath[2] = $"{_configuration["ApiBaseUrl"]}/{source.Images[2]}";
            if (source.Images.Count() > 3)
                ImagesPath[3] = $"{_configuration["ApiBaseUrl"]}/{source.Images[3]}";
        }
        return ImagesPath;
    }
}
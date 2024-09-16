using AutoMapper;
using Core.Dtos;
using Core.Entities.ProductEntities;

namespace API.Helpers;
public class ProductImagesResolver(IConfiguration configuration) : IValueResolver<Product, ProductResponse, string[]>
{
    public string[] Resolve(Product source, ProductResponse destination, string[] destMember, ResolutionContext context)
    {
        var imagesPath = new string[source.Images.Length];

        if (source.Images.Length > 0)
            imagesPath[0] = $"{configuration["ApiBaseUrl"]}/{source.Images[0]}";

        if (source.Images.Length > 1)
            imagesPath[1] = $"{configuration["ApiBaseUrl"]}/{source.Images[1]}";

        if (source.Images.Length > 2)
            imagesPath[2] = $"{configuration["ApiBaseUrl"]}/{source.Images[2]}";

        if (source.Images.Length > 3)
            imagesPath[3] = $"{configuration["ApiBaseUrl"]}/{source.Images[3]}";

        return imagesPath;
    }
}
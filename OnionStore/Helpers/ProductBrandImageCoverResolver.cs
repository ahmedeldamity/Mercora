using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace DotNetCore_ECommerce.Helpers
{
    public class ProductBrandImageCoverResolver : IValueResolver<ProductBrand, ProductBrandToReturnDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductBrandImageCoverResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(ProductBrand source, ProductBrandToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageCover))
            {
                return $"{_configuration["ApiBaseUrl"]}/{source.ImageCover}";
            }
            return string.Empty;
        }
    }
}
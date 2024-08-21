using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.BasketEntities;
using Core.Entities.Product_Entities;
using DotNetCore_ECommerce.Dtos;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles(/*IConfiguration configuration*/)
        {
            //_configuration = configuration;

            CreateMap<Product, ProductToReturnDto>()
                .ForMember(F => F.Brand, C => C.MapFrom(S => S.Brand.Name))
                .ForMember(F => F.Category, C => C.MapFrom(S => S.Category.Name))
                // -- We wanted to bring configuration to bring "ApiBaseUrl From appsetting.json 
                // -- but this isn't work because when we register automapper in program class 
                // -- it create this class with parameter less constractor 
                // -- so it will chain on the parameter less constractor and didn't see this constractor
                // -- so i commented the below line and configuration
                //.ForMember(d => d.PictureUrl, o => o.MapFrom(s => $"{_configuration["ApiBaseUrl"]}/{s.PictureUrl}"))
                // -- the solution of this issue is: instead of using MapFrom I use MapFrom<"class inherit from IValueResolver<sourse, destination, member>">
                .ForMember(d => d.ImageCover, o => o.MapFrom<ProductImageCoverResolver>())
                .ForMember(d => d.Images, o => o.MapFrom<ProductImagesResolver>());

            CreateMap<ProductBrand, ProductBrandToReturnDto>()
                .ForMember(d => d.ImageCover, o => o.MapFrom<ProductBrandImageCoverResolver>());

            CreateMap<BasketDto, Basket>();

            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<Basket, BasketToReturnDto>();

            CreateMap<BasketItem, BasketItemToReturnDto>();
        }
    }
}
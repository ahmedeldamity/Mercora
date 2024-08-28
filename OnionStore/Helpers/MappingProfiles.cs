using AutoMapper;
using Core.Dtos;
using Core.Entities;
using Core.Entities.BasketEntities;
using Core.Entities.IdentityEntities;
using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;

namespace DotNetCore_ECommerce.Helpers;
public class MappingProfiles : Profile
{
    public MappingProfiles(/*IConfiguration configuration*/)
    {
        //_configuration = configuration;

        CreateMap<Product, ProductResponse>()
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

        CreateMap<ProductBrand, ProductBrandResponse>()
            .ForMember(d => d.ImageCover, o => o.MapFrom<ProductBrandImageCoverResolver>());

        CreateMap<BasketRequest, Basket>();

        CreateMap<BasketItemRequest, BasketItem>();

        CreateMap<Basket, BasketResponse>();

        CreateMap<BasketItem, BasketItemResponse>();

        CreateMap<OrderAddressRequest, OrderAddress>().ReverseMap();

        CreateMap<Order, OrderResponse>()
                .ForMember(d => d.DeliveryMethodName, o => o.MapFrom(s => s.DeliveryMethod.Name))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

        CreateMap<OrderItem, OrderItemRequest>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ImageCover, o => o.MapFrom(s => s.Product.ProductImageCover))
                .ForMember(d => d.ImageCover, o => o.MapFrom<ProductImageCoverInOrderResolver>());

        CreateMap<UserAddressResponse, UserAddress>().ReverseMap();
    }
}
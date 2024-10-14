using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.BasketEntities;
using BlazorEcommerce.Domain.Entities.IdentityEntities;
using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.Entities.ProductEntities;
using BlazorEcommerce.Shared.Category;
using BlazorEcommerce.Shared.Product;

namespace BlazorEcommerce.Application.MappingProfıles;
public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Product, ProductResponse>()
	        .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
	        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
	        .ForMember(dest => dest.ImageCover, opt => opt.MapFrom<ProductImageCoverResolver>());

        CreateMap<ProductRequest, Product>();

        CreateMap<ProductBrand, ProductBrandResponse>()
            .ForMember(d => d.ImageCover, o => o.MapFrom<ProductBrandImageCoverResolver>());

        CreateMap<ProductBrandRequest, ProductBrand>();

        CreateMap<ProductCategory, CategoryResponse>();

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
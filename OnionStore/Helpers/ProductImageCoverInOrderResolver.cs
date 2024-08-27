using AutoMapper;
using Core.Entities.OrderEntities;
using Shared.Dtos;

namespace DotNetCore_ECommerce.Helpers;
public class ProductImageCoverInOrderResolver(IConfiguration _configuration) : IValueResolver<OrderItem, OrderItemRequest, string>
{
    public string Resolve(OrderItem source, OrderItemRequest destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.Product.ProductImageCover))
        {
            return $"{_configuration["ApiBaseUrl"]}/{source.Product.ProductImageCover}";
        }
        return string.Empty;
    }
}
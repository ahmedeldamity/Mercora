using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.OrderEntities;
using Microsoft.Extensions.Configuration;

namespace BlazorEcommerce.Application.MappingProfıles;
public class ProductImageCoverInOrderResolver(IConfiguration configuration) : IValueResolver<OrderItem, OrderItemRequest, string>
{
    public string Resolve(OrderItem source, OrderItemRequest destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.Product.ProductImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.Product.ProductImageCover}" : string.Empty;
    }
}
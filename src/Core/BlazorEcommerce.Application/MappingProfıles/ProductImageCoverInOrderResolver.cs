namespace BlazorEcommerce.Application.MappingProfıles;
public class ProductImageCoverInOrderResolver(IConfiguration configuration) : IValueResolver<OrderItem, OrderItemRequest, string>
{
    public string Resolve(OrderItem source, OrderItemRequest destination, string destMember, ResolutionContext context)
    {
        return !string.IsNullOrEmpty(source.Product.ProductImageCover) ? $"{configuration["ApiBaseUrl"]}/{source.Product.ProductImageCover}" : string.Empty;
    }
}
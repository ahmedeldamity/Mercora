namespace Core.Dtos;
public record OrderRequest
{
    public string BasketId { get; set; } = null!;
    public OrderAddressRequest ShippingAddress { get; set; } = new();
}
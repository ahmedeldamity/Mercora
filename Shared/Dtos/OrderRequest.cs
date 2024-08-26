namespace Shared.Dtos;
public class OrderRequest
{
    public string BasketId { get; set; } = null!;
    public OrderAddressRequest ShippingAddress { get; set; } = new();
}
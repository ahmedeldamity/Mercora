namespace Shared.Dtos;
public class OrderDto
{
    public string BasketId { get; set; } = null!;
    public OrderAddressDto ShippingAddress { get; set; } = new();
}
namespace Shared.Dtos;
public class OrderDto
{
    public string BasketId { get; set; } = null!;
    public int DeliveryMethodId { get; set; }
    public OrderAddressDto ShippingAddress { get; set; } = new();
}
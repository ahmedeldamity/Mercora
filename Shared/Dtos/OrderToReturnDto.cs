namespace Shared.Dtos;
public class OrderToReturnDto
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; } = null!;
    public DateTimeOffset OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public OrderAddressDto ShippingAddress { get; set; } = new();
    public string DeliveryMethodName { get; set; } = null!;
    public decimal DeliveryMethodCost { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>();
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public string PaymentIntentId { get; set; } = null!;
}
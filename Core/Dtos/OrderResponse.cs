namespace Core.Dtos;
public class OrderResponse
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; } = null!;
    public DateTimeOffset OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public OrderAddressRequest ShippingAddress { get; set; } = new();
    public string DeliveryMethodName { get; set; } = null!;
    public decimal DeliveryMethodCost { get; set; }
    public ICollection<OrderItemRequest> Items { get; set; } = new HashSet<OrderItemRequest>();
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public string PaymentIntentId { get; set; } = null!;
}
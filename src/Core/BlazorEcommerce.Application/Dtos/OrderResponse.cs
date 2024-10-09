namespace BlazorEcommerce.Application.Dtos;
public record OrderResponse(
    int Id,
    string BuyerEmail,
    DateTimeOffset OrderDate,
    string Status,
    OrderAddressRequest ShippingAddress,
    string DeliveryMethodName,
    decimal DeliveryMethodCost,
    ICollection<OrderItemRequest> Items,
    decimal Subtotal,
    decimal Total,
    string PaymentIntentId
);
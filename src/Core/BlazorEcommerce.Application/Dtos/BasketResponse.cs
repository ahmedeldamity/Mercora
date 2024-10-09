namespace BlazorEcommerce.Application.Dtos;
public record BasketResponse(
    string Id,
    List<BasketItemResponse> Items,
    int? DeliveryMethodId,
    decimal? ShippingPrice,
    string? PaymentIntentId,
    string? ClientSecret
);
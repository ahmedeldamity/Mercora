namespace BlazorEcommerce.Application.Dtos;
public record BasketRequest(
    string Id,
    List<BasketItemRequest> Items,
    int? DeliveryMethodId
);
namespace Core.Dtos;
public record BasketRequest(
    string Id,
    List<BasketItemRequest> Items,
    int? DeliveryMethodId
);
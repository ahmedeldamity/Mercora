namespace Shared.Dtos;
public class BasketRequest
{
    public string Id { get; set; } = null!;
    public List<BasketItemRequest> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
}
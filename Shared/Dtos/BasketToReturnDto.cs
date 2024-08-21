namespace Shared.Dtos;
public class BasketToReturnDto
{
    public string Id { get; set; } = null!;
    public List<BasketItemToReturnDto> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
    public decimal? ShippingPrice { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
}
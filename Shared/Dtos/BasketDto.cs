namespace Shared.Dtos;
public class BasketDto
{
    public string Id { get; set; } = null!;

    public List<BasketItemDto> Items { get; set; } = [];

    public string? PaymentIntentId { get; set; }

    public string? ClientSecret { get; set; }

    public int? DeliveryMethodId { get; set; }
    public decimal? ShippingPrice { get; set; }
}
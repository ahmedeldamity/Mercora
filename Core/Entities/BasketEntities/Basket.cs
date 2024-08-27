namespace Core.Entities.BasketEntities;
public class Basket(string id)
{
    public string Id { get; set; } = id;
    public List<BasketItem> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
    public decimal? ShippingPrice { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
}
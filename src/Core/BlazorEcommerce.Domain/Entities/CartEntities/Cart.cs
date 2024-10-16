namespace BlazorEcommerce.Domain.Entities.CartEntities;
public class Cart(string id)
{
    public string Id { get; set; } = id;
    public List<CartItem> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
    public decimal? ShippingPrice { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ClientSecret { get; set; }
}
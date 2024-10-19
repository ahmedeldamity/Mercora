namespace BlazorEcommerce.Shared.Cart;
public class CartRequest(string id)
{
	public string Id { get; set; } = id;
	public List<CartItemRequest> Items { get; init; } = [];
	public int? DeliveryMethodId { get; set; }
}
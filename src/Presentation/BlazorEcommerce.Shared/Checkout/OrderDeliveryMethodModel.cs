namespace BlazorEcommerce.Shared.Checkout;
public class OrderDeliveryMethodModel
{
	public int Id { get; set; }
	public string Name { get; set; } = null!; // the name of delivery way
	public string Description { get; set; } = null!;
	public decimal Cost { get; set; }
	public string DeliveryTime { get; set; } = null!;
}
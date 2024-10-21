using BlazorEcommerce.Shared.Checkout;

namespace BlazorEcommerce.Client.Services.CartService;
public interface ICartService
{
	public event Action OnChange;

	public CartResponse? Basket { get; set; }

	public string Message { get; set; }

	Task InitializeBasket();

	Task AddProductToBasket(ProductResponse product, decimal quantity);

	Task IncreaseItemCountInBasket(CartItemResponse item, decimal quantity);

	Task RemoveItemFromBasket(int productId, decimal quantity);

	Task ChangeDeliveryMethod(OrderDeliveryMethodModel deliveryMethodModel);

	Task CreatePaymentIntend(string basketId);
}
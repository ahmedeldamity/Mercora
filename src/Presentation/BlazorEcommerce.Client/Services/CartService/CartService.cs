namespace BlazorEcommerce.Client.Services.CartService;
public class CartService(IHttpClientFactory _httpClientFactory, ILocalStorageService localStorageService) : ICartService
{
	private readonly HttpClient httpClient = _httpClientFactory.CreateClient("Auth");

	public event Action? OnChange;

	public CartResponse? Cart { get; set; }

	public string Message { get; set; } = "Loading cart...";
	
	private void NotifyStateChanged() => OnChange?.Invoke();

	public async Task InitializeCart()
	{
		var cartId = await localStorageService.GetItemAsync<string>("cartId");

		if (string.IsNullOrWhiteSpace(cartId))
		{
			cartId = Guid.NewGuid().ToString();

			await localStorageService.SetItemAsync("cartId", cartId);
		}

		await RefreshCart(cartId);
	}

	public async Task AddProductToCart(ProductResponse product, decimal quantity)
	{
		var cartItem = MapToCartItemRequest(product, quantity);

		await AddOrUpdateItemInCart(cartItem);
	}

	public async Task IncreaseItemCountInCart(CartItemResponse item, decimal quantity)
	{
		var cartItem = MapToCartItemRequest(item, quantity);

		await AddOrUpdateItemInCart(cartItem);
	}

	private async Task AddOrUpdateItemInCart(CartItemRequest item)
	{
		var existingItem = Cart?.Items.FirstOrDefault(x => x.Id == item.Id);

		if (existingItem != null)
		{
			existingItem.CartItemQuantity += item.CartItemQuantity;
		}
		else
		{
			var itemResponse = MapToCartItemResponse(item);
			Cart?.Items.Add(itemResponse);
		}

		await UpdateCart();
	}

	public async Task RemoveItemFromCart(int productId, decimal quantity)
	{
		var item = Cart?.Items.FirstOrDefault(x => x.Id == productId);

		if (item == null) return;

		if (item.CartItemQuantity > quantity)
			item.CartItemQuantity -= quantity;
		else
			Cart?.Items.Remove(item);

		await UpdateCart();
	}

	public async Task ChangeDeliveryMethod(OrderDeliveryMethodModel deliveryMethodModel)
	{
		if(Cart == null) return;

		Cart.DeliveryMethodId = deliveryMethodModel.Id;

		Cart.ShippingPrice = deliveryMethodModel.Cost;

		await UpdateCart();
	}

	public async Task CreatePaymentIntend(string cartId)
	{
		var response = await httpClient.PostAsJsonAsync($"api/Payment/{cartId}", Cart);

		if (response.IsSuccessStatusCode)
		{
			Cart = await response.Content.ReadFromJsonAsync<CartResponse>();

			Message = "Payment created successfully.";

			Console.WriteLine("Success Man");

			NotifyStateChanged();
		}
	}

	private async Task UpdateCart()
	{
		var response = await httpClient.PostAsJsonAsync("api/Cart", Cart);

		if (!response.IsSuccessStatusCode)
		{
			Message = "Failed to update cart.";
		}

		Cart = await response.Content.ReadFromJsonAsync<CartResponse>();

		NotifyStateChanged();
	}

	private async Task RefreshCart(string cartId)
	{
		var response = await httpClient.GetAsync($"api/Cart?id={cartId}");

		if (response.IsSuccessStatusCode)
		{
			Cart = await response.Content.ReadFromJsonAsync<CartResponse>();
			Message = "Cart loaded successfully.";
		}
		else
		{
			Cart = new CartResponse(cartId);
			Message = "Failed to load cart.";
		}

		NotifyStateChanged();
	}

	private static CartItemRequest MapToCartItemRequest(ProductResponse product, decimal quantity)
	{
		return new CartItemRequest
		{
			Id = product.Id,
			Name = product.Name,
			Price = product.Price,
			CartItemQuantity = quantity,
			ImageCover = product.ImageCover,
			Description = product.Description,
			Category = product.CategoryName,
			Brand = product.BrandName,
			Quantity = product.Quantity,
			RatingsAverage = product.RatingsAverage
		};
	}

	private static CartItemRequest MapToCartItemRequest(CartItemResponse item, decimal quantity)
	{
		return new CartItemRequest
		{
			Id = item.Id,
			Name = item.Name,
			Price = item.Price,
			CartItemQuantity = quantity,
			ImageCover = item.ImageCover,
			Description = item.Description,
			Category = item.Category,
			Brand = item.Brand,
			Quantity = item.Quantity,
			RatingsAverage = item.RatingsAverage
		};
	}

	private static CartItemResponse MapToCartItemResponse(CartItemRequest item)
	{
		return new CartItemResponse
		{
			Id = item.Id,
			Name = item.Name,
			Price = item.Price,
			CartItemQuantity = item.CartItemQuantity,
			ImageCover = item.ImageCover,
			Description = item.Description,
			Category = item.Category,
			Brand = item.Brand,
			Quantity = item.Quantity,
			RatingsAverage = item.RatingsAverage
		};
	}

}
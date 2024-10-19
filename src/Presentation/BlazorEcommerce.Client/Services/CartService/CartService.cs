namespace BlazorEcommerce.Client.Services.CartService;
public class CartService(HttpClient httpClient, ILocalStorageService localStorageService) : ICartService
{
	public event Action? OnChange;

	public CartResponse? Basket { get; set; }

	public string Message { get; set; } = "Loading cart...";

	private void NotifyStateChanged() => OnChange?.Invoke();

	public async Task InitializeBasket()
	{
		var cartId = await localStorageService.GetItemAsync<string>("cartId");

		if (string.IsNullOrWhiteSpace(cartId))
		{
			cartId = Guid.NewGuid().ToString();
			await localStorageService.SetItemAsync("cartId", cartId);
		}

		await RefreshBasket(cartId);
	}

	public async Task AddProductToBasket(ProductResponse product, decimal quantity)
	{
		var cartItem = MapToCartItemRequest(product, quantity);

		await AddOrUpdateItemInBasket(cartItem);
	}

	public async Task IncreaseItemCountInBasket(CartItemResponse item, decimal quantity)
	{
		var cartItem = MapToCartItemRequest(item, quantity);

		await AddOrUpdateItemInBasket(cartItem);
	}

	private async Task AddOrUpdateItemInBasket(CartItemRequest item)
	{
		var existingItem = Basket?.Items.FirstOrDefault(x => x.Id == item.Id);

		if (existingItem != null)
		{
			existingItem.CartItemQuantity += item.CartItemQuantity;
		}
		else
		{
			var itemResponse = MapToCartItemResponse(item);
			Basket?.Items.Add(itemResponse);
		}

		await UpdateBasket();
	}

	public async Task RemoveItemFromBasket(int productId, decimal quantity)
	{
		var item = Basket?.Items.FirstOrDefault(x => x.Id == productId);

		if (item == null) return;

		if (item.CartItemQuantity > quantity)
		{
			item.CartItemQuantity -= quantity;
		}
		else
		{
			Basket?.Items.Remove(item);
		}

		await UpdateBasket();
	}

	private async Task UpdateBasket()
	{
		var response = await httpClient.PostAsJsonAsync("api/Cart", Basket);

		if (!response.IsSuccessStatusCode)
		{
			Message = "Failed to update cart.";
		}

		Basket = await response.Content.ReadFromJsonAsync<CartResponse>();

		NotifyStateChanged();
	}

	private async Task RefreshBasket(string cartId)
	{
		var response = await httpClient.GetAsync($"api/Cart?id={cartId}");

		if (response.IsSuccessStatusCode)
		{
			Basket = await response.Content.ReadFromJsonAsync<CartResponse>();
			Message = "Cart loaded successfully.";
		}
		else
		{
			Basket = new CartResponse(cartId);
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
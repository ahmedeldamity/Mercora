using BlazorEcommerce.Shared.Checkout;

namespace BlazorEcommerce.Client.Services.Checkout;
public class CheckoutService(IHttpClientFactory _httpClientFactory) : ICheckoutService
{
	private readonly HttpClient httpClient = _httpClientFactory.CreateClient("Auth");

	public async Task<List<OrderDeliveryMethodModel>> GetDeliveryMethods()
	{
		var response = await httpClient.GetFromJsonAsync<List<OrderDeliveryMethodModel>>("api/deliverymethod");

		return response ?? [];
	}
}
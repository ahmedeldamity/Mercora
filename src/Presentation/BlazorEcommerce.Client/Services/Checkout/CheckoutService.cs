using BlazorEcommerce.Shared.Checkout;

namespace BlazorEcommerce.Client.Services.Checkout;
public class CheckoutService(HttpClient httpClient): ICheckoutService
{
	public async Task<List<OrderDeliveryMethodModel>> GetDeliveryMethods()
	{
		var response = await httpClient.GetFromJsonAsync<List<OrderDeliveryMethodModel>>("api/deliverymethod");

		return response ?? [];
	}
}
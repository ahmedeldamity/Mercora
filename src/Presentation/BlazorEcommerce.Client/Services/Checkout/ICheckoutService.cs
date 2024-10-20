using BlazorEcommerce.Shared.Checkout;

namespace BlazorEcommerce.Client.Services.Checkout;
public interface ICheckoutService
{
	Task<List<OrderDeliveryMethodModel>> GetDeliveryMethods();
}
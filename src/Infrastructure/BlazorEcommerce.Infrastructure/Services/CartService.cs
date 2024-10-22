namespace BlazorEcommerce.Infrastructure.Services;
public class CartService(ICartRepository cartRepository, IDeliveryMethodService deliveryMethodService, IMapper mapper) : ICartService
{
    public async Task<Result<CartResponse>> CreateOrUpdateCartAsync(CartRequest cartRequest)
    {
        var cart = mapper.Map<CartRequest, Cart>(cartRequest);

        var createdOrUpdatedCart = await cartRepository.CreateOrUpdateCartAsync(cart);

        if (createdOrUpdatedCart is null)
        {
            return Result.Failure<CartResponse>(new Error(400, "Failed to create or update the cart. Please try again."));
        }

        var CartResponse = mapper.Map<Cart, CartResponse>(createdOrUpdatedCart);
 
        return Result.Success(CartResponse);
    }
	
    public async Task<Result<CartResponse>> GetCartAsync(string id)
    {
        var cart = await cartRepository.GetCartAsync(id);

        if (cart is null)
        {
	        var deliveryMethods = await deliveryMethodService.GetAllDeliveryMethodsAsync();

	        var cheapestDeliveryMethod = deliveryMethods?.Value.FirstOrDefault(x => x.Cost == 0);

	        if (cheapestDeliveryMethod != null)
	        {
		        cart = new Cart(id)
		        {
			        DeliveryMethodId = cheapestDeliveryMethod.Id,
			        ShippingPrice = cheapestDeliveryMethod.Cost
		        };
	        }
        }

        var CartResponse = mapper.Map<Cart, CartResponse>(cart ?? new Cart(id));

        return Result.Success(CartResponse);
    }
	
    public async Task DeleteCartAsync(string id)
    {
        await cartRepository.DeleteCartAsync(id);
    }

}
namespace BlazorEcommerce.Infrastructure.Services;
public class CartService(IBasketRepository basketRepository, IDeliveryMethodService deliveryMethodService, IMapper mapper) : ICartService
{
    public async Task<Result<CartResponse>> CreateOrUpdateCartAsync(CartRequest cartDto)
    {
        var basket = mapper.Map<CartRequest, Cart>(cartDto);

        var createdOrUpdatedBasket = await basketRepository.CreateOrUpdateBasketAsync(basket);

        if (createdOrUpdatedBasket is null)
        {
            return Result.Failure<CartResponse>(new Error(400, "Failed to create or update the basket. Please try again."));
        }

        var CartResponse = mapper.Map<Cart, CartResponse>(createdOrUpdatedBasket);

        return Result.Success(CartResponse);
    }

    public async Task<Result<CartResponse>> GetCartAsync(string id)
    {
        var basket = await basketRepository.GetBasketAsync(id);

        if (basket is null)
        {
	        var deliveryMethods = await deliveryMethodService.GetAllDeliveryMethodsAsync();

	        var cheapestDeliveryMethod = deliveryMethods?.Value.FirstOrDefault(x => x.Cost == 0);

	        if (cheapestDeliveryMethod != null)
	        {
		        basket = new Cart(id)
		        {
			        DeliveryMethodId = cheapestDeliveryMethod.Id,
			        ShippingPrice = cheapestDeliveryMethod.Cost
		        };
	        }
        }

        var CartResponse = mapper.Map<Cart, CartResponse>(basket ?? new Cart(id));

        return Result.Success(CartResponse);
    }
	
    public async Task DeleteCartAsync(string id)
    {
        await basketRepository.DeleteBasketAsync(id);
    }

}
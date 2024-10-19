namespace BlazorEcommerce.Infrastructure.Services;
public class CartService(IBasketRepository basketRepository, IMapper mapper) : ICartService
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
        var basket = await basketRepository.GetBasketAsync(id) ?? new Cart(id);

        var CartResponse = mapper.Map<Cart, CartResponse>(basket);

        return Result.Success(CartResponse);
    }
	
    public async Task DeleteCartAsync(string id)
    {
        await basketRepository.DeleteBasketAsync(id);
    }

}
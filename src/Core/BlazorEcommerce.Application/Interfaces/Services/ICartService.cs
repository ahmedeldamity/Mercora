using BlazorEcommerce.Domain.ErrorHandling;
using BlazorEcommerce.Shared.Cart;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface ICartService
{
    Task<Result<CartResponse>> CreateOrUpdateCartAsync(CartRequest cartDto);
    Task<Result<CartResponse>> GetCartAsync(string id);
    Task DeleteCartAsync(string id);
}
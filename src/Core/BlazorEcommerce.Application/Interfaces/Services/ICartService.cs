namespace BlazorEcommerce.Application.Interfaces.Services;
public interface ICartService
{
    Task<Result<CartResponse>> CreateOrUpdateCartAsync(CartRequest cartRequest);
    Task<Result<CartResponse>> GetCartAsync(string id);
    Task DeleteCartAsync(string id);
}
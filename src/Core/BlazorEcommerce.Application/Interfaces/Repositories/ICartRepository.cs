namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface ICartRepository
{
    Task<Cart?> CreateOrUpdateCartAsync(Cart cart);
    Task<Cart?> GetCartAsync(string cartId);
    Task<bool> DeleteCartAsync(string cartId);
}
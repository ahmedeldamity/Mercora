using BlazorEcommerce.Domain.Entities.CartEntities;

namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface IBasketRepository
{
    Task<Cart?> CreateOrUpdateBasketAsync(Cart cart);
    Task<Cart?> GetBasketAsync(string basketId);
    Task<bool> DeleteBasketAsync(string basketId);
}
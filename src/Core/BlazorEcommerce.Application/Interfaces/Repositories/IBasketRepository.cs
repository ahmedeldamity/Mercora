using BlazorEcommerce.Domain.Entities.BasketEntities;

namespace BlazorEcommerce.Application.Interfaces.Repositories;
public interface IBasketRepository
{
    Task<Basket?> CreateOrUpdateBasketAsync(Basket basket);
    Task<Basket?> GetBasketAsync(string basketId);
    Task<bool> DeleteBasketAsync(string basketId);
}
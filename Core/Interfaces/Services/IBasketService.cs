using Shared.Dtos;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IBasketService
{
    Task<Result<BasketResponse>> CreateOrUpdateBasketAsync(BasketRequest basketDto);
    Task<Result<BasketResponse>> GetBasketAsync(string id);
    Task DeleteBasketAsync(string id);
}
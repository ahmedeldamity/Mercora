using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.ErrorHandling;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IBasketService
{
    Task<Result<BasketResponse>> CreateOrUpdateBasketAsync(BasketRequest basketDto);
    Task<Result<BasketResponse>> GetBasketAsync(string id);
    Task DeleteBasketAsync(string id);
}
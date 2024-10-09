using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Domain.Entities.BasketEntities;
using BlazorEcommerce.Domain.ErrorHandling;

namespace BlazorEcommerce.Infrastructure.Services;
public class BasketService(IBasketRepository basketRepository, IMapper mapper) : IBasketService
{
    public async Task<Result<BasketResponse>> CreateOrUpdateBasketAsync(BasketRequest basketDto)
    {
        var basket = mapper.Map<BasketRequest, Basket>(basketDto);

        var createdOrUpdatedBasket = await basketRepository.CreateOrUpdateBasketAsync(basket);

        if (createdOrUpdatedBasket is null)
        {
            return Result.Failure<BasketResponse>(new Error(400, "Failed to create or update the basket. Please try again."));
        }

        var basketResponse = mapper.Map<Basket, BasketResponse>(createdOrUpdatedBasket);

        return Result.Success(basketResponse);
    }

    public async Task<Result<BasketResponse>> GetBasketAsync(string id)
    {
        var basket = await basketRepository.GetBasketAsync(id) ?? new Basket(id);

        var basketResponse = mapper.Map<Basket, BasketResponse>(basket);

        return Result.Success(basketResponse);
    }

    public async Task DeleteBasketAsync(string id)
    {
        await basketRepository.DeleteBasketAsync(id);
    }

}
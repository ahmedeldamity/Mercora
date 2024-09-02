using AutoMapper;
using Core.Dtos;
using Core.Entities.BasketEntities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class BasketService(IBasketRepository _basketRepository, IMapper _mapper) : IBasketService
{
    public async Task<Result<BasketResponse>> CreateOrUpdateBasketAsync(BasketRequest basketDto)
    {
        var basket = _mapper.Map<BasketRequest, Basket>(basketDto);

        var createdOrUpdatedBasket = await _basketRepository.CreateOrUpdateBasketAsync(basket);

        if (createdOrUpdatedBasket is null)
        {
            return Result.Failure<BasketResponse>(new Error(400, "Failed to create or update the basket. Please try again."));
        }

        var basketResponse = _mapper.Map<Basket, BasketResponse>(createdOrUpdatedBasket);

        return Result.Success(basketResponse);
    }

    public async Task<Result<BasketResponse>> GetBasketAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id) ?? new Basket(id);

        var basketResponse = _mapper.Map<Basket, BasketResponse>(basket);

        return Result.Success(basketResponse);
    }

    public async Task DeleteBasketAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }

}
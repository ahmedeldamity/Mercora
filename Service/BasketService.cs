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

        var createdOrUpdated = await _basketRepository.CreateOrUpdateBasketAsync(basket);

        if (createdOrUpdated is null) 
            return Result.Failure<BasketResponse>(new Error("Basket could not be created or updated", 400));

        var basketResponse = _mapper.Map<Basket, BasketResponse>(createdOrUpdated);

        return Result.Success(basketResponse);
    }

    public async Task<Result<BasketResponse>> GetBasketAsync(string id)
    {
        var basket = await _basketRepository.GetBasketAsync(id);

        if (basket is null)
            basket = new Basket(id);

        var basketResponse = _mapper.Map<Basket, BasketResponse>(basket);

        return Result.Success(basketResponse);
    }

    public async Task DeleteBasketAsync(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}
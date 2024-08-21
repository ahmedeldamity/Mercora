using API.Errors;
using AutoMapper;
using Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Core.Entities.BasketEntities;
using Core.Interfaces.Repositories;

namespace API.Controllers;
public class BasketController(IBasketRepository _basketRepository, IMapper _mapper) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<BasketToReturnDto>> CreateOrUpdateBasket(BasketDto basketDto)
    {
        var basket = _mapper.Map<BasketDto, Basket>(basketDto);

        var createdOrUpdated = await _basketRepository.CreateOrUpdateBasketAsync(basket);

        if (createdOrUpdated is null) return BadRequest(new ApiResponse(400));

        return Ok(_mapper.Map<Basket, BasketToReturnDto>(createdOrUpdated));
    }
}
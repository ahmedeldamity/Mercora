using API.Extensions;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class BasketController(IBasketService _basketService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<BasketResponse>> CreateOrUpdateBasket(BasketRequest basketDto)
    {
        var result = await _basketService.CreateOrUpdateBasketAsync(basketDto);

        return result.IsSuccess ? result.ToSuccess() : result.ToProblemOrSuccessMessage();
    }

    [HttpGet]
    public async Task<ActionResult<BasketResponse>> GetBasket(string id)
    {
        var result = await _basketService.GetBasketAsync(id);

        return result.ToSuccess();
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await _basketService.DeleteBasketAsync(id);
    }

}
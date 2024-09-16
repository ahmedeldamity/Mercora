using API.Extensions;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class BasketController(IBasketService basketService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<BasketResponse>> CreateOrUpdateBasket(BasketRequest basketDto)
    {
        var result = await basketService.CreateOrUpdateBasketAsync(basketDto);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<ActionResult<BasketResponse>> GetBasket(string id)
    {
        var result = await basketService.GetBasketAsync(id);

        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await basketService.DeleteBasketAsync(id);
    }

}
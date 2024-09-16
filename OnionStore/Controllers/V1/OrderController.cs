using API.Extensions;
using API.Helpers;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[Authorize]
public class OrderController(IOrderService orderService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult> CreateOrder(OrderRequest orderDto)
    {
        var result = await orderService.CreateOrderAsync(orderDto.BasketId, orderDto.ShippingAddress);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetOrdersForUser()
    {
        var result = await orderService.GetOrdersForUserAsync();

        return Ok(result.Value);
    }

    [HttpGet("{orderId:int}")]
    [Cached(600)]
    public async Task<ActionResult<OrderResponse>> GetSpecificOrderForUser(int orderId)
    {
        var result = await orderService.GetSpecificOrderForUserAsync(orderId);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
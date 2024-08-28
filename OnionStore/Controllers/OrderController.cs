﻿using Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using API.Extensions;

namespace API.Controllers;

[Authorize]
public class OrderController(IOrderService _orderService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult> CreateOrder(OrderRequest orderDto)
    {
        var result = await _orderService.CreateOrderAsync(orderDto.BasketId, orderDto.ShippingAddress);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetOrdersForUser()
    {
        var result = await _orderService.GetOrdersForUserAsync();

        return Ok(result.Value);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> GetSpecificOrderForUser(int orderId)
    {
        var result = await _orderService.GetSpecificOrderForUserAsync(orderId);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
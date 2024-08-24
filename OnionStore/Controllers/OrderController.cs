using API.Errors;
using AutoMapper;
using Core.Entities.OrderEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class OrderController(IMapper _mapper, IOrderService _orderService) : BaseController
{
    [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
    {
        var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

        var address = _mapper.Map<OrderAddressDto, OrderAddress>(orderDto.ShippingAddress);

        var order = await _orderService.CreateOrderAsync(buyerEmail!, orderDto.BasketId, address);

        if (order is null)
            return BadRequest(new ApiResponse(400));

        return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
    }
}
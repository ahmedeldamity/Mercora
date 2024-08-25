using API.Errors;
using AutoMapper;
using Shared.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Core.Entities.OrderEntities;
using Microsoft.AspNetCore.Authorization;

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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
    {
        var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

        var orders = await _orderService.GetOrdersForUserAsync(buyerEmail!);

        return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderToReturnDto>> GetSpecificOrderForUser(int orderId)
    {
        var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

        var order = await _orderService.GetSpecificOrderForUserAsync(orderId, buyerEmail!);

        if (order is null)
            return NotFound(new ApiResponse(404));

        return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
    }

    [HttpGet("deliveryMethod")]
    public async Task<ActionResult<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethods()
    {
        var deliveryMethods = await _orderService.GetAllDeliveryMethodsAsync();

        return Ok(deliveryMethods);
    }
}
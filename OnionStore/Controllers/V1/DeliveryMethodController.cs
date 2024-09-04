using API.Extensions;
using API.Helpers;
using Core.Entities.OrderEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;
public class DeliveryMethodController(IDeliveryMethodService _deliveryMethodService) : BaseController
{
    [HttpGet]
    [Cached(600)]
    public async Task<ActionResult<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethods()
    {
        var result = await _deliveryMethodService.GetAllDeliveryMethodsAsync();

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [Cached(600)]
    public async Task<ActionResult<OrderDeliveryMethod>> GetDeliveryMethodById(int id)
    {
        var result = await _deliveryMethodService.GetDeliveryMethodByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<ActionResult<OrderDeliveryMethod>> CreateDeliveryMethod(OrderDeliveryMethod deliveryMethod)
    {
        var result = await _deliveryMethodService.CreateDeliveryMethodAsync(deliveryMethod);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDeliveryMethod>> UpdateDeliveryMethod(int id, OrderDeliveryMethod deliveryMethod)
    {
        var result = await _deliveryMethodService.UpdateDeliveryMethodAsync(id, deliveryMethod);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<OrderDeliveryMethod>> DeleteDeliveryMethod(int id)
    {
        var result = await _deliveryMethodService.DeleteDeliveryMethodAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}
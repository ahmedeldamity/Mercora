using API.Extensions;
using Core.Entities.OrderEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class DeliveryMethodController(IDeliveryMethodService _deliveryMethodService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethods()
    {
        var result = await _deliveryMethodService.GetAllDeliveryMethodsAsync();

        return Ok(result.Value);
    }

}
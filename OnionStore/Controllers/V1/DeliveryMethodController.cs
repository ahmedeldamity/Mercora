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

}
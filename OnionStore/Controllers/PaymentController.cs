using API.Errors;
using Core.Entities.BasketEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class PaymentController(IPaymentService _paymentService) : BaseController
{
    [ProducesResponseType(typeof(Basket), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [HttpPost("{basketId}")]
    [Authorize]
    public async Task<ActionResult<Basket>> CreateOrUpdatePaymentIntend(string basketId)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

        if (basket is null)
            return BadRequest(new ApiResponse(400, "An error with your basket"));

        return Ok(basket);
    }
}
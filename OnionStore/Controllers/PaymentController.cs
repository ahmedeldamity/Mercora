using API.Extensions;
using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers;
public class PaymentController(IPaymentService _paymentService, ILogger<PaymentController> _logger) : BaseController
{
    private const string _webhookSecret = "whsec_f7cb2a38fa3f766b411c6184763756a8c944a4f0cf869208b10e3153c3dc5962";

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<Basket>> CreateOrUpdatePaymentIntend(string basketId)
    {
        var result = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webhookSecret);

        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

        Order order;

        switch (stripeEvent.Type)
        {
            case Events.PaymentIntentSucceeded:
                order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                _logger.LogInformation("Payment Is Succeeded. Order ID: {OrderId}", order?.Id);
                break;

            case Events.PaymentIntentPaymentFailed:
                order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                _logger.LogError("Payment Is Failed. Order ID: {OrderId}", order?.Id);
                break;
        }
        return Ok();
    }

}
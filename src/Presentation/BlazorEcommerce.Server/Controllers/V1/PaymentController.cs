using Stripe;

namespace BlazorEcommerce.Server.Controllers.V1;
public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger) : BaseController
{
    private const string WebhookSecret = "whsec_f7cb2a38fa3f766b411c6184763756a8c944a4f0cf869208b10e3153c3dc5962";

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<Cart>> CreateOrUpdatePaymentIntend(string basketId)
    {
        var result = await paymentService.CreateOrUpdatePaymentIntent(basketId);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WebhookSecret);

        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

        Order order;

        switch (stripeEvent.Type)
        {
            case Events.PaymentIntentSucceeded:
                order = await paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                logger.LogInformation("Payment Is Succeeded. Order ID: {OrderId}", order?.Id);
                break;

            case Events.PaymentIntentPaymentFailed:
                order = await paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                logger.LogError("Payment Is Failed. Order ID: {OrderId}", order?.Id);
                break;
        }
        return Ok();
    }

}
namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IPaymentService
{
    Task<Result<CartResponse>> CreateOrUpdatePaymentIntent(string cartId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
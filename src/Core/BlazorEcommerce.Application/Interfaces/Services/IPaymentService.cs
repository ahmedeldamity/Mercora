using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.ErrorHandling;
using BlazorEcommerce.Shared.Cart;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IPaymentService
{
    Task<Result<CartResponse>> CreateOrUpdatePaymentIntent(string basketId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
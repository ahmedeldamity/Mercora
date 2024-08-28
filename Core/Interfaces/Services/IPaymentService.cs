using Core.Dtos;
using Core.Entities.OrderEntities;
using Core.ErrorHandling;

namespace Core.Interfaces.Services;
public interface IPaymentService
{
    Task<Result<BasketResponse>> CreateOrUpdatePaymentIntent(string basketId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
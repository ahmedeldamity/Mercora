using Core.Entities.OrderEntities;
using Shared.Dtos;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IPaymentService
{
    Task<Result<BasketResponse>> CreateOrUpdatePaymentIntent(string basketId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
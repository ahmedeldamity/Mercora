using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;

namespace Core.Interfaces.Services;
public interface IPaymentService
{
    Task<Basket?> CreateOrUpdatePaymentIntent(string basketId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
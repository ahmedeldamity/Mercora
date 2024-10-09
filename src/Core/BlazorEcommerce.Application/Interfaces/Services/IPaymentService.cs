using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.ErrorHandling;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IPaymentService
{
    Task<Result<BasketResponse>> CreateOrUpdatePaymentIntent(string basketId);
    Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool succeeded);
}
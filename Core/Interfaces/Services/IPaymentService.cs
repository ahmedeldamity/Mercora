using Core.Entities.BasketEntities;

namespace Core.Interfaces.Services;
public interface IPaymentService
{
    Task<Basket?> CreateOrUpdatePaymentIntent(string basketId);
}
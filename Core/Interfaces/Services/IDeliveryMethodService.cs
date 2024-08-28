using Core.Entities.OrderEntities;
using Core.ErrorHandling;

namespace Core.Interfaces.Services;
public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync();
}
using Core.Entities.OrderEntities;
using Core.ErrorHandling;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync();
}
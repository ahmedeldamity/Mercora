using Core.Entities.OrderEntities;
using Shared.Helpers;

namespace Core.Interfaces.Services;
public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync();
}
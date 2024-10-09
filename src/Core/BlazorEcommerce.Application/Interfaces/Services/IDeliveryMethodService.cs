using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.ErrorHandling;

namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyList<OrderDeliveryMethod>>> GetAllDeliveryMethodsAsync();
    Task<Result<OrderDeliveryMethod>> GetDeliveryMethodByIdAsync(int id);
    Task<Result<OrderDeliveryMethod>> CreateDeliveryMethodAsync(OrderDeliveryMethod deliveryMethod);
    Task<Result<OrderDeliveryMethod>> UpdateDeliveryMethodAsync(int id, OrderDeliveryMethod deliveryMethod);
    Task<Result<OrderDeliveryMethod>> DeleteDeliveryMethodAsync(int id);
}
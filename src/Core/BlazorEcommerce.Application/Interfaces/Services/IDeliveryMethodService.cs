namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyList<OrderDeliveryMethodModel>>> GetAllDeliveryMethodsAsync();
    Task<Result<OrderDeliveryMethodModel>> GetDeliveryMethodByIdAsync(int id);
    Task<Result<OrderDeliveryMethodModel>> CreateDeliveryMethodAsync(OrderDeliveryMethodModel deliveryMethod);
    Task<Result<OrderDeliveryMethodModel>> UpdateDeliveryMethodAsync(int id, OrderDeliveryMethodModel deliveryMethod);
    Task<Result<OrderDeliveryMethodModel>> DeleteDeliveryMethodAsync(int id);
}
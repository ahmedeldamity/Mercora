using Core.Entities.OrderEntities;

namespace Core.Interfaces.Services;
public interface IOrderService
{
    Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, OrderAddress shippingAddress);
    Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
    Task<Order?> GetSpecificOrderForUserAsync(int orderId, string buyerEmail);
    Task<IReadOnlyList<OrderDeliveryMethod>> GetAllDeliveryMethodsAsync();
}
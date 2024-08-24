using Core.Entities.OrderEntities;

namespace Core.Interfaces.Services;
public interface IOrderService
{
    Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, OrderAddress shippingAddress);
}
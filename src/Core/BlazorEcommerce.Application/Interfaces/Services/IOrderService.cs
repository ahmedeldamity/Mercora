namespace BlazorEcommerce.Application.Interfaces.Services;
public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(string cartId, OrderAddressRequest orderAddress);
    Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync();
    Task<Result<OrderResponse>> GetSpecificOrderForUserAsync(int orderId);
}
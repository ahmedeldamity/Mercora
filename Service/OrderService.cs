using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Service;
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepository;

    public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository)
    {
        _unitOfWork = unitOfWork;
        _basketRepository = basketRepository;
    }
    public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, OrderAddress shippingAddress)
    {
        // 1. Get basket from basket repository
        var basket = await _basketRepository.GetBasketAsync(basketId);

        // 2. Get Items at Basket from Product repository for get the real products price
        var orderitems = new List<OrderItem>();

        if (basket?.Items?.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                var productItemOrdered = new ProductOrderItem(item.Id, product!.Name, product.ImageCover);

                var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                orderitems.Add(orderItem);
            }
        }

        // 3. Calculate SubTotal
        var subTotal = orderitems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

        // 4. Get Delivery Method
        var deliveryMethod = await _unitOfWork.Repository<OrderDeliveryMethod>().GetByIdAsync(deliveryMethodId);

        // 5. Create order
        var orderRepository = _unitOfWork.Repository<Order>();

        var order = new Order(buyerEmail, shippingAddress, deliveryMethod!, orderitems, subTotal);

        await orderRepository.AddAsync(order);

        // 6. SaveChanges()
        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return null;

        return order;
    }
}
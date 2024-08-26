using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.OrderSpecifications;

namespace Service;
public class OrderService(IUnitOfWork _unitOfWork, IBasketRepository _basketRepository) : IOrderService
{
    #region Why We Take OrderAddress
    // In my thinking before, I was thinking to take buyerEmail and bring user address from database
    // but this is not good idea because it is not always the user take the order to his address
    // it can be take the order to another address like whrn he buy gift to another person
    #endregion
    public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, OrderAddress shippingAddress)
    {
        // 1. Get basket from basket repository
        var basket = await _basketRepository.GetBasketAsync(basketId);

        if(basket is null || basket.DeliveryMethodId is null || basket.PaymentIntentId is null)
            return null;

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
        var deliveryMethod = await _unitOfWork.Repository<OrderDeliveryMethod>().GetByIdAsync(basket!.DeliveryMethodId.Value);

        // 5. Check if exist order in database has the same Payment Intent will update it else will create new one

        var orderRepository = _unitOfWork.Repository<Order>();

        var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);

        var order = await orderRepository.GetByIdWithSpecAsync(orderSpec);

        if (order is not null)  // Exist one before so we will update it
        {
            order.ShippingAddress = shippingAddress;
            order.DeliveryMethod = deliveryMethod!;
            order.SubTotal = subTotal;
            orderRepository.UpdateAsync(order);
        }
        else    // Create New Order
        {
            order = new Order(buyerEmail, shippingAddress, deliveryMethod!, orderitems, subTotal, basket.PaymentIntentId);

            await orderRepository.AddAsync(order);
        }

        // 6. SaveChanges()
        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return null;

        return order;
    }
    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    {
        var ordersRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(buyerEmail);

        var orders = await ordersRepo.GetAllWithSpecAsync(spec);

        return orders;
    }
    public async Task<Order?> GetSpecificOrderForUserAsync(int orderId, string buyerEmail)
    {
        var ordersRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(buyerEmail, orderId);

        var order = await ordersRepo.GetByIdWithSpecAsync(spec);

        return order;
    }
    public async Task<IReadOnlyList<OrderDeliveryMethod>> GetAllDeliveryMethodsAsync()
    {
        var deliveryMethodsRepo = _unitOfWork.Repository<OrderDeliveryMethod>();

        var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();

        return deliveryMethods;
    }
}
using AutoMapper;
using Core.Entities.OrderEntities;
using Core.Entities.Product_Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.OrderSpecifications;
using Microsoft.AspNetCore.Http;
using Shared.Dtos;
using Shared.Helpers;
using System.Security.Claims;

namespace Service;
public class OrderService(IUnitOfWork _unitOfWork, IBasketRepository _basketRepository, IMapper _mapper, IHttpContextAccessor _httpContextAccessor) : IOrderService
{
    #region Why We Take OrderAddress
    // In my thinking before, I was thinking to take buyerEmail and bring user address from database
    // but this is not good idea because it is not always the user take the order to his address
    // it can be take the order to another address like whrn he buy gift to another person
    #endregion
    public async Task<Result<OrderResponse>> CreateOrderAsync(string basketId, OrderAddressRequest orderAddress)
    {
        var buyerEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var address = _mapper.Map<OrderAddressRequest, OrderAddress>(orderAddress);

        // 1. Get basket from basket repository
        var basket = await _basketRepository.GetBasketAsync(basketId);

        if (basket is null || basket.DeliveryMethodId is null || basket.PaymentIntentId is null)
            return Result.Failure<OrderResponse>(new Error("Invalid Basket", 404));

        // 2. Get Items at Basket from Product repository for get the real products price
        var orderitems = new List<OrderItem>();

        if (basket?.Items?.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetEntityAsync(item.Id);

                var productItemOrdered = new ProductOrderItem(item.Id, product!.Name, product.ImageCover);

                var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                orderitems.Add(orderItem);
            }
        }

        // 3. Calculate SubTotal
        var subTotal = orderitems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

        // 4. Get Delivery Method
        var deliveryMethod = await _unitOfWork.Repository<OrderDeliveryMethod>().GetEntityAsync(basket!.DeliveryMethodId.Value);

        // 5. Check if exist order in database has the same Payment Intent will update it else will create new one

        var orderRepository = _unitOfWork.Repository<Order>();

        var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);

        var order = await orderRepository.GetEntityAsync(orderSpec);

        if (order is not null)  // Exist one before so we will update it
        {
            order.ShippingAddress = address;

            order.DeliveryMethod = deliveryMethod!;

            order.SubTotal = subTotal;

            orderRepository.Update(order);
        }
        else    // Create New Order
        {
            order = new Order(buyerEmail!, address, deliveryMethod!, orderitems, subTotal, basket.PaymentIntentId);

            await orderRepository.AddAsync(order);
        }

        // 6. SaveChanges()
        var result = await _unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<OrderResponse>(new Error("Invalid Basket", 404));

        var orderRespone = _mapper.Map<Order, OrderResponse>(order);

        return Result.Success(orderRespone);
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync()
    {
        var buyerEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var ordersRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(buyerEmail!);

        var orders = await ordersRepo.GetAllAsync(spec);

        var ordersResponse = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderResponse>>(orders);

        return Result.Success(ordersResponse);
    }

    public async Task<Result<OrderResponse>> GetSpecificOrderForUserAsync(int orderId)
    {
        var buyerEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var ordersRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(buyerEmail!, orderId);

        var order = await ordersRepo.GetEntityAsync(spec);

        if (order is null)
            return Result.Failure<OrderResponse>(new Error("Order not found", 404));

        var orderResponse = _mapper.Map<Order, OrderResponse>(order);

        return Result.Success(orderResponse);
    }

}
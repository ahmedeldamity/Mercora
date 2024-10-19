namespace BlazorEcommerce.Infrastructure.Services;
public class OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IOrderService
{
    #region Why We Take OrderAddress
    // In my thinking before, I was thinking to take buyerEmail and bring user address from database
    // but this is not good idea because it is not always the user take the order to his address
    // it can be takes the order to another address like where he buys gift to another person
    #endregion
    public async Task<Result<OrderResponse>> CreateOrderAsync(string basketId, OrderAddressRequest orderAddress)
    {
        var userEmail = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var address = mapper.Map<OrderAddressRequest, OrderAddress>(orderAddress);

        // 1. Get basket from basket repository
        var basket = await basketRepository.GetBasketAsync(basketId);

        if (basket?.DeliveryMethodId is null || basket.PaymentIntentId is null)
            return Result.Failure<OrderResponse>(new Error(404, "Invalid basket data. Ensure that the basket, delivery method, and payment intent are properly provided."));

        // 2. Get Items at Cart from Product repository for get the real products price
        var orderItems = new List<OrderItem>();

        if (basket?.Items?.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.Repository<Product>().GetEntityAsync(item.Id);

                var productItemOrdered = new ProductOrderItem(item.Id, product!.Name, product.ImageCover);

                var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                orderItems.Add(orderItem);
            }
        }

        // 3. Calculate SubTotal
        var subTotal = orderItems.Sum(orderItem => orderItem.Price * orderItem.Quantity);

        // 4. Get Delivery Method
        var deliveryMethod = await unitOfWork.Repository<OrderDeliveryMethod>().GetEntityAsync(basket!.DeliveryMethodId.Value);

        // 5. Check if exist order in database has the same Payment Intent will update it else will create new one

        var orderRepository = unitOfWork.Repository<Order>();

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
            order = new Order(userEmail!, address, deliveryMethod!, orderItems, subTotal, basket.PaymentIntentId);

            await orderRepository.AddAsync(order);
        }

        // 6. SaveChanges()
        var result = await unitOfWork.CompleteAsync();

        if (result <= 0)
            return Result.Failure<OrderResponse>(new Error(400, "Failed to complete the order creation process. Please try again or contact support if the issue persists."));

        var orderResponse = mapper.Map<Order, OrderResponse>(order);

        return Result.Success(orderResponse);
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync()
    {
        var userEmail = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var ordersRepo = unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(userEmail);

        var orders = await ordersRepo.GetAllAsync(spec);

        var ordersResponse = mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderResponse>>(orders);

        return Result.Success(ordersResponse);
    }

    public async Task<Result<OrderResponse>> GetSpecificOrderForUserAsync(int orderId)
    {
        var userEmail = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        var ordersRepo = unitOfWork.Repository<Order>();

        var spec = new OrderSpecification(userEmail, orderId);

        var order = await ordersRepo.GetEntityAsync(spec);

        if (order is null)
            return Result.Failure<OrderResponse>(new Error(404, "The requested order was not found. Please verify the order details and try again."));

        var orderResponse = mapper.Map<Order, OrderResponse>(order);

        return Result.Success(orderResponse);
    }

}
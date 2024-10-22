using Stripe;
using Product = BlazorEcommerce.Domain.Entities.ProductEntities.Product;

namespace BlazorEcommerce.Infrastructure.Services;
public class PaymentService(IUnitOfWork unitOfWork, ICartRepository cartRepository, IConfiguration configuration, IMapper mapper) : IPaymentService
{
    public async Task<Result<CartResponse>> CreateOrUpdatePaymentIntent(string cartId)
    {
        // ser secret key of stripe
        StripeConfiguration.ApiKey = configuration["StripeSettings:Secretkey"];

        // get amount of salary from basket
        var cart = await cartRepository.GetCartAsync(cartId);

        if (cart is null)
            return Result.Failure<CartResponse>(new Error(404, "The specified basket could not be found. Please check the basket details and try again."));

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<OrderDeliveryMethod>().GetEntityAsync(cart.DeliveryMethodId.Value);

            if (deliveryMethod is null)
            {
                return Result.Failure<CartResponse>(new Error(404, "The specified delivery method could not be found. Please check the basket details and try again."));
            }

            cart.ShippingPrice = deliveryMethod.Cost;
        }

        if (cart?.Items.Count > 0)
        {
            foreach (var item in cart.Items)
            {
                var product = await unitOfWork.Repository<Product>().GetEntityAsync(item.Id);

                if (product is null)
                {
                    return Result.Failure<CartResponse>(new Error(404, $"Product with ID {item.Id} was not found. Please review your basket."));
                }

                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }
        }

        PaymentIntentService paymentIntentService = new();

        PaymentIntent paymentIntent;

        var itemsTotal = cart!.Items.Sum(item => (long)(item.Price * item.Quantity * 100));

        var shippingTotal = (long)(cart.ShippingPrice! * 100);

        if (string.IsNullOrEmpty(cart?.PaymentIntentId)) // -> create new payment intent
        {
            var createOptions = new PaymentIntentCreateOptions()
            {
                Amount = itemsTotal + shippingTotal,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };

            paymentIntent = await paymentIntentService.CreateAsync(createOptions);

            cart!.PaymentIntentId = paymentIntent.Id;

            cart.ClientSecret = paymentIntent.ClientSecret;
        }
        else // -> update payment intent
        {
            var updateOptions = new PaymentIntentUpdateOptions()
            {
                Amount = itemsTotal + shippingTotal,
            };

            paymentIntent = await paymentIntentService.UpdateAsync(cart.PaymentIntentId, updateOptions);
        }

        await cartRepository.CreateOrUpdateCartAsync(cart);

        var CartResponse = mapper.Map<Cart, CartResponse>(cart);

        return Result.Success(CartResponse);
    }

    public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
    {
        var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

        var order = await unitOfWork.Repository<Order>().GetEntityAsync(spec);

        order!.Status = isSucceeded ? OrderStatus.PaymentSucceeded : OrderStatus.PaymentFailed;

        unitOfWork.Repository<Order>().Update(order);

        await unitOfWork.CompleteAsync();

        return order;
    }

}
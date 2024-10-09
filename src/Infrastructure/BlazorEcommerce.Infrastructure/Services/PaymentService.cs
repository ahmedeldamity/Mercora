using AutoMapper;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Repositories;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Application.Specifications.OrderSpecifications;
using BlazorEcommerce.Domain.Entities.BasketEntities;
using BlazorEcommerce.Domain.Entities.OrderEntities;
using BlazorEcommerce.Domain.ErrorHandling;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = BlazorEcommerce.Domain.Entities.ProductEntities.Product;

namespace BlazorEcommerce.Infrastructure.Services;
public class PaymentService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IConfiguration configuration, IMapper mapper) : IPaymentService
{
    public async Task<Result<BasketResponse>> CreateOrUpdatePaymentIntent(string basketId)
    {
        // ser secret key of stripe
        StripeConfiguration.ApiKey = configuration["StripeSettings:Secretkey"];

        // get amount of salary from basket
        var basket = await basketRepository.GetBasketAsync(basketId);

        if (basket is null)
            return Result.Failure<BasketResponse>(new Error(404, "The specified basket could not be found. Please check the basket details and try again."));

        if (basket.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await unitOfWork.Repository<OrderDeliveryMethod>().GetEntityAsync(basket.DeliveryMethodId.Value);

            if (deliveryMethod is null)
            {
                return Result.Failure<BasketResponse>(new Error(404, "The specified delivery method could not be found. Please check the basket details and try again."));
            }

            basket.ShippingPrice = deliveryMethod.Cost;
        }

        if (basket?.Items.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                var product = await unitOfWork.Repository<Product>().GetEntityAsync(item.Id);

                if (product is null)
                {
                    return Result.Failure<BasketResponse>(new Error(404, $"Product with ID {item.Id} was not found. Please review your basket."));
                }

                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }
        }

        PaymentIntentService paymentIntentService = new();

        PaymentIntent paymentIntent;

        var itemsTotal = basket!.Items.Sum(item => (long)(item.Price * item.Quantity * 100));

        var shippingTotal = (long)(basket.ShippingPrice! * 100);

        if (string.IsNullOrEmpty(basket?.PaymentIntentId)) // -> create new payment intent
        {
            var createOptions = new PaymentIntentCreateOptions()
            {
                Amount = itemsTotal + shippingTotal,
                Currency = "usd",
                PaymentMethodTypes = ["card"]
            };

            paymentIntent = await paymentIntentService.CreateAsync(createOptions);

            basket!.PaymentIntentId = paymentIntent.Id;

            basket.ClientSecret = paymentIntent.ClientSecret;
        }
        else // -> update payment intent
        {
            var updateOptions = new PaymentIntentUpdateOptions()
            {
                Amount = itemsTotal + shippingTotal,
            };

            paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updateOptions);
        }

        await basketRepository.CreateOrUpdateBasketAsync(basket);

        var basketResponse = mapper.Map<Basket, BasketResponse>(basket);

        return Result.Success(basketResponse);
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
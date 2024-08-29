using AutoMapper;
using Core.Dtos;
using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;
using Core.ErrorHandling;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Specifications.OrderSpecifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product_Entities.Product;

namespace Service;
public class PaymentService(IUnitOfWork _unitOfWork, IBasketRepository _basketRepository, IConfiguration _configuration, IMapper _mapper) : IPaymentService
{
    public async Task<Result<BasketResponse>> CreateOrUpdatePaymentIntent(string basketId)
    {
        // ser secret key of stripe
        StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

        // get amount of salary from basket
        var basket = await _basketRepository.GetBasketAsync(basketId);

        if (basket is null)
            return Result.Failure<BasketResponse>(404, "Basket not found");

        if (basket.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await _unitOfWork.Repository<OrderDeliveryMethod>().GetEntityAsync(basket.DeliveryMethodId.Value);

            if (deliveryMethod is null)
                return Result.Failure<BasketResponse>(404, "Delivery method not found");

            basket.ShippingPrice = deliveryMethod.Cost;
        }

        if (basket?.Items.Count > 0)
        {
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetEntityAsync(item.Id);

                if (product is null)
                    return Result.Failure<BasketResponse>(404, "Product not found");

                if (item.Price != product.Price)
                    item.Price = product.Price;
            }
        }

        PaymentIntentService paymentIntentService = new PaymentIntentService();

        PaymentIntent paymentIntent;

        long itemsTotal = basket!.Items.Sum(item => (long)(item.Price * item.Quantity * 100));

        long shippingTotal = (long)(basket.ShippingPrice! * 100);

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

        await _basketRepository.CreateOrUpdateBasketAsync(basket);

        var basketResponse = _mapper.Map<Basket, BasketResponse>(basket);

        return Result.Success(basketResponse);
    }

    public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
    {
        var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

        var order = await _unitOfWork.Repository<Order>().GetEntityAsync(spec);

        if (isSucceeded)
            order!.Status = OrderStatus.paymentSucceeded;
        else
            order!.Status = OrderStatus.paymentFailed;

        _unitOfWork.Repository<Order>().Update(order);

        await _unitOfWork.CompleteAsync();

        return order;
    }

}
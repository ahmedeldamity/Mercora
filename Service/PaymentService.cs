using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product_Entities.Product;

namespace Service
{
    public class PaymentService(IUnitOfWork _unitOfWork, IBasketRepository _basketRepository, IConfiguration _configuration) : IPaymentService
    {
        public async Task<Basket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            // ser secret key of stripe
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];

            // get amount of salary from basket
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return null;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<OrderDeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

                if (deliveryMethod is null)
                    return null;

                basket.ShippingPrice = deliveryMethod.Cost;
            }

            if (basket?.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                    if (product is null)
                        return null;

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

            return basket;
        }
    }
}
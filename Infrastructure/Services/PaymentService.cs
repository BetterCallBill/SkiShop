using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            // Set up Stripe
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            
            var basket = await _basketRepository.GetBasketAsync(basketId);
            var shippingPrice = 0m;
            
            // Check if there is a delivery method in basket
            if (basket.DeliveryMethodId.HasValue)
            {
                // Get delivery method price
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }
            
            // Check each item in the basket 
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                
                // If the price is different
                if (productItem.Price != item.Price)
                {
                    item.Price = productItem.Price;
                }
            }
            
            // Use Stripe service
            var service = new PaymentIntentService();
            
            PaymentIntent paymentIntent;
            
            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    // Convert decimal to long
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                
                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update
                var options = new PaymentIntentUpdateOptions
                {
                    // Convert decimal to long
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100
                };
                
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            
            // Update basket
            await _basketRepository.UpdateBasketAsync(basket);
            
            return basket;
        }
    }
}
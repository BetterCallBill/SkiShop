using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly string _webhookSecret;

        public PaymentsController(
            IPaymentService paymentService, 
            ILogger<PaymentsController> logger,
            IConfiguration config)
        {
            _paymentService = paymentService;
            _logger = logger;
            _webhookSecret = config.GetSection("StripeSettings:WebhookSecret").Value;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null)
            {
                return BadRequest(new ApiResponse(400, "Problem with your basket"));
            }

            return basket;
        }

        // Stripe webhook
        // https://stripe.com/docs/webhooks#webhooks-def
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            // read data from Stripe
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _webhookSecret);

            // Handle the event
            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                // Then define and call a method to handle the successful payment intent.
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogInformation("Payment Succeeded: ", paymentIntent.Id);
                var order = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                _logger.LogInformation("Order updated to payment received: ", order.Id);
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogError("Payment Failed: ", paymentIntent.Id);
                var order = await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                _logger.LogInformation("Order updated to payment failed: ", order.Id);
            }

            return new EmptyResult();
        }
    }
}
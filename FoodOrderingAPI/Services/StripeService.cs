using FoodOrderingAPI.Models;
using Stripe;

namespace FoodOrderingAPI.Services
{
    public class StripeService : IStripeService
    {
        public ApplicationDBContext DBContext { get; }
        private string stripeKey = "sk_test_51RnFfeRxaBxeNU6sg87fDZWBd37Ssyrk6WsZ8ZS6w69SwZ7yxYaMZxJklKfkLtLnsy6OZA0ay2dEjkQrXqkjlH7b00P9ZiIWxf";
        public StripeService(ApplicationDBContext dBContext)
        {
            DBContext = dBContext;
            StripeConfiguration.ApiKey = stripeKey;
        }

        public async Task<string> CreateProductStripeAsync(Item item)
        {
            var productOptions = new ProductCreateOptions
            {
                Name = item.Name,
                Description = item.Description,
            };
            var productService = new ProductService();
            var product = await productService.CreateAsync(productOptions);
            return product.Id;
        }

        public async Task UpdateProductStripeAsync(Item item)
        {
            // Update the product details in Stripe
            var options = new ProductUpdateOptions
            {
                Name = item.Name,
                Description = item.Description
            };
            var service = new ProductService();
            Product product = await service.UpdateAsync(item.StripeProductId, options);
        }

        public async Task DeleteProductStripeAsync(Item item)
        {
            var options = new ProductUpdateOptions { Active = false };
            var service = new ProductService();
            Product product = await service.UpdateAsync(item.StripeProductId, options);
        }

        public async Task<string> CreatePriceStripeAsync(Item item, string productId, decimal discount)
        {
            var priceOptions = new PriceCreateOptions
            {
                UnitAmount = (long)((item.Price * 100) * (1 - discount / 100)), // Convert to cents
                Currency = "egp",
                Product = productId
            };
            var priceService = new PriceService();
            var price = await priceService.CreateAsync(priceOptions);
            return price.Id;
        }

        public async Task DeletePriceStripeAsync(Item item)
        {
            // Deactivate the old price
            var priceOptions = new PriceUpdateOptions { Active = false };
            var priceService = new PriceService();
            Price price = await priceService.UpdateAsync(item.StripePriceId, priceOptions);
        }
    }
}

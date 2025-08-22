using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IStripeService
    {
        public Task<string> CreateProductStripeAsync(Item item);
        public Task<string> CreateDeliveryFeeStripeAsync(Restaurant restaurant);
        public Task UpdateProductStripeAsync(Item item);
        public Task DeleteProductStripeAsync(Item item);
        public Task<string> CreatePriceStripeAsync(Item item, string productId, decimal discount);
        public Task<string> CreateDeliveryFeePriceStripeAsync(Restaurant restaurant, string productId);
        public Task DeletePriceStripeAsync(Item item);
        public Task DeleteDeliveryFeePriceStripeAsync(Restaurant restaurant);
        public string CreatePaymentLink(List<ShoppingCartItem> items, Restaurant restaurant);
    }
}

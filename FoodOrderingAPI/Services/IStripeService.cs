using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IStripeService
    {
        public Task<string> CreateProductStripeAsync(Item item);
        public Task UpdateProductStripeAsync(Item item);
        public Task DeleteProductStripeAsync(Item item);
        public Task<string> CreatePriceStripeAsync(Item item, string productId, decimal discount);
        public Task DeletePriceStripeAsync(Item item);
        public string CreatePaymentLink(List<ShoppingCartItem> items);
    }
}

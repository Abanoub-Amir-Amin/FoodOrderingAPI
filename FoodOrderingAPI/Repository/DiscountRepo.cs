using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class DiscountRepo:IDiscountRepo
    {
        private readonly ApplicationDBContext _context;

        public IStripeService StripeService { get; }

        public DiscountRepo(ApplicationDBContext context, IStripeService stripeService)
        {
            _context = context;
            StripeService = stripeService;
        }
        // ===== Discounts CRUD =====
        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            _context.Discounts.Add(discount);
            var item = _context.Items.FirstOrDefault(i => i.ItemID == discount.ItemID);
            item.DiscountedPrice = item.Price * (1 - discount.Percentage / 100);
            await StripeService.DeletePriceStripeAsync(item); // Deactivate the old price
            var priceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, discount.Percentage); // Create a new price in Stripe for the discount
            item.StripePriceId = priceId; // Update the Stripe Price ID in the Item entity
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            var item = _context.Items.FirstOrDefault(i => i.ItemID == discount.ItemID);
            item.DiscountedPrice = item.Price * (1 - discount.Percentage / 100);
            await StripeService.DeletePriceStripeAsync(item); // Deactivate the old price
            var priceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, discount.Percentage); // Create a new price in Stripe for the discount
            item.StripePriceId = priceId; // Update the Stripe Price ID in the Item entity
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(int discountId)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.DiscountID == discountId);
            if (discount == null) return false;

            _context.Discounts.Remove(discount);
            var item = _context.Items.FirstOrDefault(i => i.ItemID == discount.ItemID);
            item.DiscountedPrice = item.Price;
            await StripeService.DeletePriceStripeAsync(item); // Deactivate the old price
            item.StripePriceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, 0); // Create a new price in Stripe without discount
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

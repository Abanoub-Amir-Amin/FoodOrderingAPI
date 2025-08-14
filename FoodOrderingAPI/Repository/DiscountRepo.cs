using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class DiscountRepo : IDiscountRepo
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
            if (item == null)
                throw new InvalidOperationException($"Item with ID {discount.ItemID} not found.");

            item.DiscountedPrice = item.Price * (1 - discount.Percentage / 100);

            await StripeService.DeletePriceStripeAsync(item); // Deactivate the old price
            var priceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, discount.Percentage); // Create new price in Stripe
            item.StripePriceId = priceId;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return discount;
        }


        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemID == discount.ItemID);
            if (item != null)
            {
                item.DiscountedPrice = item.Price * (1 - discount.Percentage / 100);
                await StripeService.DeletePriceStripeAsync(item);
                var priceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, discount.Percentage);
                item.StripePriceId = priceId;
                _context.Items.Update(item);
            }
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

        public async Task<IEnumerable<Discount>> GetDiscountsByRestaurantAsync(string restaurantId)
        {
            return await _context.Discounts
                .Include(d => d.Item)
                .Where(d => d.Item.RestaurantID == restaurantId)
                .ToListAsync();
        }

        public async Task<Discount> GetDiscountByIDAsync(int discountId)
        {
            return await _context.Discounts.FirstOrDefaultAsync(d => d.DiscountID == discountId);
        }
    }
}

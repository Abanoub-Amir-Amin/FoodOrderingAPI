using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class DiscountRepo:IDiscountRepo
    {
        private readonly ApplicationDBContext _context;

        public DiscountRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        // ===== Discounts CRUD =====
        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(int discountId, string restaurantId)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.DiscountID == discountId && d.RestaurantID == restaurantId);
            if (discount == null) return false;

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

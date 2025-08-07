using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IDiscountService
    {
        //Discount-CRUD
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int discountId, string restaurantId);
    }
}

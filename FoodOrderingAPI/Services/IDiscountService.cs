using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IDiscountService
    {
        //Discount-CRUD
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(int discountId, DiscountDto dis);
        Task<bool> DeleteDiscountAsync(int discountId);
        Task<IEnumerable<Discount>> GetDiscountsByRestaurantAsync(string restaurantId);
        Task<Discount> GetDiscountByIDAsync(int discountId);
    }
}


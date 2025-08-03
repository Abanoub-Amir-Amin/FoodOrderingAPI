using AutoMapper;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class DiscountService:IDiscountService
    {
        private readonly IDiscountRepo _repository;
        private readonly UserManager<User> _userManager;


        public DiscountService(IDiscountRepo repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _repository = repository;
            _userManager = userManager;
        }
        //Discount-CRUD
        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            return await _repository.AddDiscountAsync(restaurantId, discount);
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            return await _repository.UpdateDiscountAsync(discount);
        }

        public async Task<bool> DeleteDiscountAsync(int discountId, string restaurantId)
        {
            return await _repository.DeleteDiscountAsync(discountId, restaurantId);
        }
    }
}

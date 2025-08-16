using AutoMapper;
using FoodOrderingAPI.Controllers;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepo _repository;
        private readonly UserManager<User> _userManager;
        private readonly IItemRepo _itemRepo;
        private readonly IMapper _mapper;


        public DiscountService(IDiscountRepo repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment, IItemRepo itemRepo)
        {
            _repository = repository;
            _userManager = userManager;
            _itemRepo = itemRepo;
            _mapper = mapper;
        }
        //Discount-CRUD
        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            return await _repository.AddDiscountAsync(restaurantId, discount);
        }

        public async Task<Discount> GetDiscountByIDAsync(int discountId)
        {
            if (discountId == null)
                throw new ArgumentException("discountId is invalid", nameof(discountId));
            return await _repository.GetDiscountByIDAsync(discountId);
        }

        public async Task<Discount> UpdateDiscountAsync(int discountId, DiscountDto dis)
        {
            var existingDiscount = await _repository.GetDiscountByIDAsync(discountId);
            if (existingDiscount == null)
                return null;

            // Update only if the property has value (not null)
            if (dis.Percentage.HasValue)
            {
                existingDiscount.Percentage = dis.Percentage.Value;
            }

            if (dis.StartDate.HasValue)
            {
                existingDiscount.StartDate = dis.StartDate.Value;
            }

            if (dis.EndDate.HasValue)
            {
                existingDiscount.EndDate = dis.EndDate.Value;
            }

            // Update ItemName if provided in dto
            if (!string.IsNullOrEmpty(dis.ItemName))
            {
                var item = await _itemRepo.GetItemByIdAsync(existingDiscount.ItemID);
                if (item != null)
                {
                    item.Name = dis.ItemName;
                    await _itemRepo.UpdateItemAsync(item);
                }
            }

            return await _repository.UpdateDiscountAsync(existingDiscount);
        }

        public async Task<bool> DeleteDiscountAsync(int discountId)
        {
            return await _repository.DeleteDiscountAsync(discountId);
        }

        public async Task<IEnumerable<Discount>> GetDiscountsByRestaurantAsync(string restaurantId)
        {
            return await _repository.GetDiscountsByRestaurantAsync(restaurantId);
        }



    }
}

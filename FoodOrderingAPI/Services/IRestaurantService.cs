using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public interface IRestaurantService
    {
        //Item-CRUD
        Task<Item> AddItemAsync(string restaurantId, ItemDto dto);
        Task<Item> UpdateItemAsync(string restaurantId, Guid itemId, ItemDto dto);
        Task<bool> DeleteItemAsync(Guid itemId, string restaurantId);
        Task<Item> GetItemByIdAsync(string restaurantId, Guid itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category);

        //Discount-CRUD
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int discountId, string restaurantId);

        //PromoCode-CRUD
        Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode);
        Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId);
        Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId);
        Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code);

        ////Restaurant Apply to Join
        Task<Restaurant> ApplyToJoinAsync(RestaurantDto dto, IFormFile logoFile = null);

        //updating restaurant itself
        Task<Restaurant> GetRestaurantByIdAsync(string userId);
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();

        Task<Restaurant> UpdateRestaurantProfileAsync(string restaurantId, RestaurantUpdateDto dto);

        //Order-CRUD
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, string[] statuses);
        Task<IEnumerable<ItemDto>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10);
        Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId);

        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);

        //Image Upload
        Task<string> SaveImageAsync(IFormFile file);

    }



}


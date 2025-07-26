using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public interface IRestaurantService
    {
        Task<Item> AddItemAsync(string restaurantId, ItemDto dto);
        //Task<Item> UpdateItemAsync(string restaurantId, ItemDto dto);
        Task<bool> DeleteItemAsync(Guid itemId, string restaurantId);
        Task<Item> GetItemByIdAsync(string restaurantId, Guid itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category);
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int discountId, string restaurantId);

        Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode);
        Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId);
        Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId);
        Task<Restaurant> ApplyToJoinAsync(RestaurantDto dto);
        Task<Restaurant> GetRestaurantByIdAsync(string userId);
        Task<Restaurant> UpdateRestaurantProfileAsync(string restaurantId, RestaurantProfileDto dto);
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, string[] statuses);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);
        Task<IEnumerable<ItemDto>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10);
    }



}


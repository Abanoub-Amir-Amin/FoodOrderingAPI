using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IRestaurantRepository
    {
        //Item-CRUD
        Task<Item> AddItemAsync(string restaurantId, Item item);
        Task<Item> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(Guid itemId, string restaurantId);
        Task<Item> GetItemByIdAsync(Guid itemId, string restaurantId);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category);
        //Task<IEnumerable<Item>> GetItemsByRestaurantAsync(string restaurantId);

        //Discount-CRUD
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int discountId, string restaurantId);

        //PromoCode-CRUD
        Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode);
        Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId);

        //Order-update
        Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId);
        Task<Restaurant> GetRestaurantByIdAsync(string userId);
        Task<Restaurant> ApplyToJoinAsync(Restaurant restaurantEntity);

        // updating restaurant itself
        Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);

        //Getting All Orders
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);
        Task<List<(Item Item, int TotalQuantity)>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10);
    }
}

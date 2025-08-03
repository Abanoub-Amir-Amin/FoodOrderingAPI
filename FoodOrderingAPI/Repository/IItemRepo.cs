using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IItemRepo
    {
        //Item-CRUD
        Task<Item> AddItemAsync(string restaurantId, Item item);
        Task<Item> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(Guid itemId);
        Task<List<Item>> GetAllItemsAsync();
        Task<Item> GetItemByIdAsync(Guid itemId);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(string category);
        Task<IEnumerable<Item>> GetItemsByRestaurantAsync(string restaurantName);
        Task<List<(Item Item, int TotalQuantity)>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10);
    }
}

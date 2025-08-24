using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IItemService
    {
        //Item-CRUD
        Task<Item> AddItemAsync(string restaurantId, ItemUpdateDto dto);
        public Task CreateItemAsync(Item item);

        Task<Item> UpdateItemAsync(Guid itemId, ItemUpdateDto dto);
        Task<bool> DeleteItemAsync(Guid itemId);
        Task<List<Item>> GetAllItemsAsync();
        Task<Item> GetItemByIdAsync(Guid itemId);
        Task<IEnumerable<Item>> GetItemsByRestaurantAsync(string restaurantName);
        Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category);
        Task<IEnumerable<ItemDto>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10);
        //Image Upload
        Task<string> SaveImageAsync(IFormFile file);

        Task<List<string>> GetAllCategoriesAsync();
    }
}

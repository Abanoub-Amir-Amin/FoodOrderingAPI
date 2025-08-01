using FoodOrderingAPI.DTO;

namespace FoodOrderingAPI.Interfaces
{
    public interface IItemRepo
    {
        public Task CreateItemAsync(string restaurantId, ItemDto item);
    }
}

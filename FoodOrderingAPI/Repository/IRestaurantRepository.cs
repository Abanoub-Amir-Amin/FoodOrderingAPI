using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IRestaurantRepository
    {
        
        //Restaurant Apply to Join
        Task<Restaurant> ApplyToJoinAsync(Restaurant restaurantEntity);

        // updating restaurant itself
        Task<Restaurant> GetRestaurantByIdAsync(string userId);
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
        Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);

    }
}

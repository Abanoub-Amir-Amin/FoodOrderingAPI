using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public interface IRestaurantService
    {
       
        ////Restaurant Apply to Join
        Task<Restaurant> ApplyToJoinAsync(RestaurantDto dto, IFormFile logoFile = null);

        //updating restaurant itself
        Task<Restaurant> GetRestaurantByIdAsync(string userId);
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();

        Task<Restaurant> UpdateRestaurantProfileAsync(string restaurantId, RestaurantUpdateDto dto);

        //Image Upload
        Task<string> SaveImageAsync(IFormFile file);
    }

}


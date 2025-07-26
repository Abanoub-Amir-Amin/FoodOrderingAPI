using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;

namespace FoodOrderingAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;

        public AdminService(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive)
        {
            return await _repository.GetRestaurantsByActiveStatusAsync(isActive);
        }

        public async Task ActivateRestaurantAsync(string restaurantId)
        {
            var restaurant = await _repository.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found");

            restaurant.IsActive = true;
            await _repository.UpdateRestaurantAsync(restaurant);
        }

        public async Task DeactivateRestaurantAsync(string restaurantId)
        {
            var restaurant = await _repository.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found");

            restaurant.IsActive = false;
            await _repository.UpdateRestaurantAsync(restaurant);
        }

        public async Task DeleteRestaurantAsync(string restaurantId)
        {
            await _repository.DeleteRestaurantAsync(restaurantId);
        }

        public async Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync()
        {
            return await _repository.GetAllDeliveryMenAsync();
        }

        public async Task DeleteDeliveryManAsync(string deliveryManId)
        {
            await _repository.DeleteDeliveryManAsync(deliveryManId);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _repository.GetAllCustomerAsync();
        }
    }


}

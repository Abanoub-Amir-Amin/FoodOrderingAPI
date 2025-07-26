using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public interface IAdminRepository
    {
        Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive);
        Task<Restaurant> GetRestaurantByIdAsync(string restaurantId);
        Task UpdateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(string restaurantId);

        Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync();
        Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId);
        Task DeleteDeliveryManAsync(string deliveryManId);
        Task<IEnumerable<Customer>> GetAllCustomerAsync();
    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive);
        Task ActivateRestaurantAsync(string restaurantId);
        Task DeactivateRestaurantAsync(string restaurantId);
        Task DeleteRestaurantAsync(string restaurantId);

        Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync();
        Task DeleteDeliveryManAsync(string deliveryManId);
        Task<IEnumerable<Customer>> GetAllCustomerAsync();
    }

}

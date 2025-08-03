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
        Task<Restaurant> GetRestaurantByUserNameAsync(string userName);
        Task UpdateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(string restaurantId);

        Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync();
        Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId);
        Task DeleteDeliveryManAsync(string deliveryManId);
        Task<IEnumerable<Customer>> GetAllCustomerAsync();

        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin> GetAdminByUserNameAsync(string UserName);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);

        Task UpdateAdminAsync(Admin admin);

    }
}

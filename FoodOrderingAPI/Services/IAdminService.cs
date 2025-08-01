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
        Task ActivateRestaurantAsync(string userName);
        Task DeactivateRestaurantAsync(string userName);
        Task DeleteRestaurantAsync(string restaurantId);

        Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync();
        Task DeleteDeliveryManAsync(string deliveryManId);
        Task<IEnumerable<Customer>> GetAllCustomerAsync();

        Task<IEnumerable<CustomerDTO>> GetCustomersOrderSummaryAsync();

        Task<IEnumerable<Admin>> GetAllAdminsAsync();

        Task<Admin> GetAdminByUserNameAsync(string UserName);

        Task<IEnumerable<Order>> GetAllOrdersAsync(string status = null); // null == all status orders

        Task UpdateAdminAsync(AdminDto dto);
    }

}

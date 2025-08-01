using AutoMapper;
using FoodOrderingAPI.Controllers;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace FoodOrderingAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository repository, IMapper mapper, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive)
        {
            return await _repository.GetRestaurantsByActiveStatusAsync(isActive);
        }

        public async Task ActivateRestaurantAsync(string userName)
        {
            var restaurant = await _repository.GetRestaurantByUserNameAsync(userName);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found");

            restaurant.IsActive = true;
            await _repository.UpdateRestaurantAsync(restaurant);
        }

        public async Task DeactivateRestaurantAsync(string userName)
        {
            var restaurant = await _repository.GetRestaurantByUserNameAsync(userName);
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

        public async Task<IEnumerable<CustomerDTO>> GetCustomersOrderSummaryAsync()
        {
            var customers = await _repository.GetAllCustomerAsync();

            var result = new List<CustomerDTO>();

            foreach (var customer in customers)
            {
                var orders = await _repository.GetOrdersByCustomerIdAsync(customer.UserID);

                var totalOrders = orders.Count();
                var deliveredOrders = orders.Count(o => o.Status == "Delivered");
                var cancelledOrders = orders.Count(o => o.Status == "Cancelled");

                var inProcessOrders = _mapper.Map<List<OrderDto>>(orders.Where(o => o.Status == "In Process").ToList());

                var customerDto = _mapper.Map<CustomerDTO>(customer);

                customerDto.TotalOrders = totalOrders;
                customerDto.TotalDeliveredOrders = deliveredOrders;
                customerDto.TotalCancelledOrders = cancelledOrders;
                customerDto.InProcessOrders = inProcessOrders;

                result.Add(customerDto);
            }

            return result;
        }


        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _repository.GetAllAdminsAsync();
        }

        public async Task<Admin> GetAdminByUserNameAsync(string UserName)
        {
            return await _repository.GetAdminByUserNameAsync(UserName);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string status = null)
        {
            if (string.IsNullOrEmpty(status))
                return await _repository.GetAllOrdersAsync();

            return await _repository.GetOrdersByStatusAsync(status);
        }

        public async Task UpdateAdminAsync(AdminDto dto)
        {
            var admin = await _repository.GetAdminByUserNameAsync(dto.User.UserName);
            if (admin == null) throw new KeyNotFoundException("Admin not found");

            // Update related User info (like Email)
            var user = admin.User;
            if (user == null) throw new Exception("User info missing");

            await _repository.UpdateAdminAsync(admin);
        }

    }


}

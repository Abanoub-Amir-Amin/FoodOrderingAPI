using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDBContext _context;

        public AdminRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive)
        {
            return await _context.Restaurants
                .Include(r => r.User)
                .Where(r => r.IsActive == isActive)
                .ToListAsync();
        }

        public async Task<Restaurant> GetRestaurantByIdAsync(string restaurantId)
        {
            return await _context.Restaurants
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RestaurantID == restaurantId);
        }

        public async Task UpdateRestaurantAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRestaurantAsync(string restaurantId)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);
            if (restaurant != null)
            {
                if (restaurant.User != null)
                {
                    _context.Users.Remove(restaurant.User);
                }

                _context.Restaurants.Remove(restaurant);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync()
        {
            return await _context.DeliveryMen
                .Include(d => d.User)
                .ToListAsync();
        }

        public async Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId)
        {
            return await _context.DeliveryMen
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DeliveryManID == deliveryManId);
        }

        public async Task DeleteDeliveryManAsync(string deliveryManId)
        {
            var deliveryMan = await GetDeliveryManByIdAsync(deliveryManId);
            if (deliveryMan != null)
            {
                if (deliveryMan.User != null)
                {
                    _context.Users.Remove(deliveryMan.User);
                }
                _context.DeliveryMen.Remove(deliveryMan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _context.Customers
                .Include(d => d.User)
                .ToListAsync();
        }
    }

}

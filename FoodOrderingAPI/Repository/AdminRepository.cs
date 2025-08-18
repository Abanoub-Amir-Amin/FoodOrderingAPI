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

        /////Restaurants//////
        public async Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive)
        {
            return await _context.Restaurants
                .Include(r => r.User)
                .Where(r => r.IsActive == isActive)
                .ToListAsync();
        }

        public async Task<Restaurant> GetRestaurantByUserNameAsync(string userName)
        {
            return await _context.Restaurants
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.User.UserName == userName);
        }

        public async Task UpdateRestaurantAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRestaurantAsync(string userName)
        {
            var restaurant = await GetRestaurantByUserNameAsync(userName);
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



        /////DeliveryMen//////
        public async Task<IEnumerable<DeliveryMan>> GetDeliveryMenByActiveStatusAsync(AccountStatusEnum accountStatus)
        {
            return (IEnumerable<DeliveryMan>)await _context.DeliveryMen
                .Include(r => r.User)
                .Where(r => r.AccountStatus == accountStatus)
                .ToListAsync();
        }

        public async Task<DeliveryMan> GetDeliveryMenByUserNameAsync(string userName)
        {
            return await _context.DeliveryMen
                .Include(d => d.User)
                .FirstOrDefaultAsync(r => r.User.UserName == userName);
        }

        public async Task UpdateDeliveryManAsync(DeliveryMan deliveryMan)
        {
            _context.DeliveryMen.Update(deliveryMan);
            await _context.SaveChangesAsync();
        }

        //public async Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync()
        //{
        //    return await _context.DeliveryMen
        //        .Include(d => d.User)
        //        .ToListAsync();
        //}

        public async Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId)
        {
            return await _context.DeliveryMen
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DeliveryManID == deliveryManId);
        }

        public async Task DeleteDeliveryManAsync(string userName)
        {
            var deliveryMan = await GetDeliveryMenByUserNameAsync(userName);
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


        /////Customers//////

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _context.Customers
                .Include(d => d.User)
                .ToListAsync();
        }
        public async Task UpdateAdminAsync(Admin admin)
        {
            _context.Admins.Update(admin);

            if (admin.User != null)
            {
                _context.Users.Update(admin.User);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<Admin> GetAdminByUserNameAsync(string UserName)
        {
            return await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.UserName == UserName);
        }


        /////Admins//////
        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _context.Admins
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerUserNameAsync(string userName)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Restaurant.User.UserName == userName)
                .ToListAsync();
        }


        /////Orders//////

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ToListAsync();

        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(StatusEnum status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .ToListAsync();
        }



    }

}

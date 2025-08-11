using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDBContext _context;

        public RestaurantRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        // ===== Restaurant Apply to Join =====
        public async Task<Restaurant> ApplyToJoinAsync(Restaurant restaurant)
        {
            if (restaurant == null)
                throw new ArgumentNullException(nameof(restaurant));
            if (restaurant.User == null)
                throw new ArgumentException("User info must be provided");
            if (string.IsNullOrWhiteSpace(restaurant.User.Email))
                throw new ArgumentException("User Email must be provided before Save.");

            restaurant.IsActive = false;

            _context.Restaurants.Add(restaurant);

            await _context.SaveChangesAsync();

            return restaurant;
        }

        // ===== Restaurant Profile =====
        public async Task<Restaurant> GetRestaurantByIdAsync(string userId)
        {
            if (userId == string.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            return await _context.Restaurants
                   .Include(r => r.User)
                   .FirstOrDefaultAsync(r => r.UserId == userId);
        }
        public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
        {
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();
            return restaurant;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _context.Restaurants.Where(r => r.IsActive)
                .Include(r => r.User)
                .ToListAsync();
        }

    }
}


    //        //Depugging
    //        Console.WriteLine("Tracked entities:");
    //            foreach (var entry in _context.ChangeTracker.Entries())
    //            {
    //                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

    //                if (entry.Entity is User u)
    //                {
    //                    Console.WriteLine($"  User Email: '{u.Email}'");
    //                }
    //}






using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDBContext _context;

        public IStripeService StripeService { get; }

        public RestaurantRepository(ApplicationDBContext context, IStripeService stripeService)
        {
            _context = context;
            StripeService = stripeService;
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
            var productId = await StripeService.CreateDeliveryFeeStripeAsync(restaurant);
            var priceId = await StripeService.CreateDeliveryFeePriceStripeAsync(restaurant, productId); // 0 is discount percentage for new items
            restaurant.StripePriceId = priceId; // Store the Stripe Price ID in the Item entity
            restaurant.StripeProductId = productId; // Store the Stripe Product ID in the Item entity
            _context.Restaurants.Add(restaurant);

            await _context.SaveChangesAsync();

            return restaurant;
        }

        // ===== Restaurant Profile =====
        public async Task<Restaurant> GetRestaurantByIdAsync(string userId)
        {
            if (userId == string.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            var restaurant = await _context.Restaurants
                   .Include(r => r.User)
                   .FirstOrDefaultAsync(r => r.UserId == userId);
            var ranking = await _context.Reviews
                .Where(r => r.RestaurantID == restaurant.RestaurantID)
                .AverageAsync(r => (float?)r.Rating) ?? 0;
            restaurant.Rating = (float)Math.Round(ranking, 1);
            return restaurant;
        }
        public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
        {
            await _context.SaveChangesAsync();
            await StripeService.DeleteDeliveryFeePriceStripeAsync(restaurant);
            var priceId = await StripeService.CreateDeliveryFeePriceStripeAsync(restaurant, restaurant.StripeProductId);
            restaurant.StripePriceId = priceId;
            return restaurant;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            var restaurants = await _context.Restaurants.Where(r => r.IsActive)
                .Include(r => r.User)
                .ToListAsync();
            foreach (var restaurant in restaurants)
            {
                var ranking = await _context.Reviews
                    .Where(r => r.RestaurantID == restaurant.RestaurantID)
                    .AverageAsync(r => (float?)r.Rating) ?? 0;
                restaurant.Rating = (float)Math.Round(ranking, 1);
            }
            return restaurants;
        }


    }
}





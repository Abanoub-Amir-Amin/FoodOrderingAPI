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

        // ===== Items CRUD =====
        public async Task<Item> AddItemAsync(string restaurantId, Item item)
        {
            item.RestaurantID = restaurantId;
            _context.Items.Add(item);           
            await _context.SaveChangesAsync();  
            return item;                      
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            _context.Items.Update(item);        
            await _context.SaveChangesAsync();  
            return item;                        
        }

        public async Task<bool> DeleteItemAsync(Guid itemId, string restaurantId)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.ItemID == itemId && i.RestaurantID == restaurantId);
            if (item == null)
                return false;                 

            _context.Items.Remove(item);        
            await _context.SaveChangesAsync(); 
            return true;                      
        }

        public async Task<Item> GetItemByIdAsync(Guid itemId, string restaurantId)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.ItemID == itemId && i.RestaurantID == restaurantId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(string restaurantId, string category)
        {
            return await _context.Items
                .Where(i => i.RestaurantID == restaurantId && i.Category == category && i.IsAvailable)
                .ToListAsync();
        }

        //public async Task<IEnumerable<Item>> GetItemsByRestaurantAsync(string restaurantId)
        //{
        //    return await _context.Items
        //        .Where(i => i.RestaurantID == restaurantId)
        //        .ToListAsync();
        //}


        // ===== Discounts CRUD =====
        public async Task<Discount> AddDiscountAsync(string restaurantId, Discount discount)
        {
            discount.RestaurantID = restaurantId;
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(int discountId, string restaurantId)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.DiscountID == discountId && d.RestaurantID == restaurantId);
            if (discount == null) return false;

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }


        // ===== Promo Codes CRUD =====
        public async Task<PromoCode> AddPromoCodeAsync(string restaurantId, PromoCode promoCode)
        {
            promoCode.RestaurantID = restaurantId;
            _context.PromoCodes.Add(promoCode);
            await _context.SaveChangesAsync();
            return promoCode;
        }

        public async Task<PromoCode> UpdatePromoCodeAsync(PromoCode promoCode)
        {
            _context.PromoCodes.Update(promoCode);
            await _context.SaveChangesAsync();
            return promoCode;
        }

        public async Task<bool> DeletePromoCodeAsync(Guid promoCodeId, string restaurantId)
        {
            var promoCode = await _context.PromoCodes
                .FirstOrDefaultAsync(p => p.PromoCodeID == promoCodeId && p.RestaurantID == restaurantId);
            if (promoCode == null) return false;

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PromoCode>> GetAllPromoCodesByRestaurantAsync(string restaurantId)
        {
            return await _context.PromoCodes
                .Where(p => p.RestaurantID.ToString() == restaurantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PromoCode>> SearchPromoCodesByCodeAsync(string restaurantId, string code)
        {
            return await _context.PromoCodes
                .Where(p => p.RestaurantID.ToString() == restaurantId && p.Code.Contains(code))
                .ToListAsync();
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


        // ===== Orders =====
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _context.Orders
                .Where(o => o.RestaurantID == restaurantId)
                .ToListAsync();
        }

        /*queries the database to find the top 10 most ordered items (topCount defaults to 10) for a given restaurant. 
          It returns a list of tuples where each tuple contains:
            **The Item entity, representing the menu item.
            **The total quantity ordered of that item (TotalQuantity).
         */
        public async Task<List<(Item Item, int TotalQuantity)>> GetMostOrderedItemsAsync(string restaurantId, int topCount = 10)
        {
            var mostOrderedItems = await _context.OrderItems
                .Where(oi => oi.Order.RestaurantID == restaurantId)
                .GroupBy(oi => oi.Item)
                .Select(g => new
                {
                    Item = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topCount)
                .ToListAsync();

            // Convert to List of Tuple<Item, int>
            return mostOrderedItems.Select(x => (x.Item, x.TotalQuantity)).ToList();
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == orderId && o.RestaurantID == restaurantId);
            if (order == null)
                return null;

            order.Status = status;
            await _context.SaveChangesAsync();
            return order;
        }


        // ===== Dashboard Summary =====
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            var orders = await GetAllOrdersByRestaurantAsync(restaurantId);

            var deliveredCount = orders.Count(o => o.Status == "Delivered");
            var inProcessCount = orders.Count(o => o.Status == "Preparing" || o.Status == "Out for Delivery");
            var cancelledCount = orders.Count(o => o.Status == "Cancelled");

            return new DashboardSummaryDto
            {
                DeliveredOrders = deliveredCount,
                InProcessOrders = inProcessCount,
                CancelledOrders = cancelledCount
            };
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






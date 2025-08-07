using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class ItemRepo : IItemRepo
    {
        public ApplicationDBContext _context { get; }

        public ItemRepo(ApplicationDBContext dBContext)
        {
            _context = dBContext;
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

        public async Task<bool> DeleteItemAsync(Guid itemId)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.ItemID == itemId);
            if (item == null)
                return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                .Where(i => i.IsAvailable)
                .ToListAsync();
        }

        public async Task<Item> GetItemByIdAsync(Guid itemId)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.ItemID == itemId);
        }

        public async Task<IEnumerable<Item>> GetItemsByCategoryAsync(string category)
        {
            return await _context.Items
                .Where(i => i.Category == category && i.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItemsByRestaurantAsync(string restaurantName)
        {
            var restaurantId = await _context.Restaurants
                .Where(r => r.RestaurantName.Contains(restaurantName))
                .Select(r => r.RestaurantID)
                .FirstOrDefaultAsync();
            return await _context.Items
                .Where(i => i.RestaurantID == restaurantId)
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
    }
}

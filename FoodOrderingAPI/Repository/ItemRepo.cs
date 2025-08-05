using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace FoodOrderingAPI.Repository
{
    public class ItemRepo : IItemRepo
    {
        public ApplicationDBContext _context { get; }
        public IStripeService StripeService { get; }

        public ItemRepo(ApplicationDBContext dBContext, IStripeService stripeService)
        {
            _context = dBContext;
            StripeService = stripeService;
        }
        // ===== Items CRUD =====
        public async Task<Item> AddItemAsync(string restaurantId, Item item)
        {
            var productId = await StripeService.CreateProductStripeAsync(item);
            var priceId = await StripeService.CreatePriceStripeAsync(item, productId, 0); // 0 is discount percentage for new items
            item.StripePriceId = priceId; // Store the Stripe Price ID in the Item entity
            item.StripeProductId = productId; // Store the Stripe Product ID in the Item entity
            item.RestaurantID = restaurantId;
            item.DiscountedPrice = item.Price; // Initialize DiscountedPrice to Price
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            await StripeService.UpdateProductStripeAsync(item);
            // Deactivate the old price
            await StripeService.DeletePriceStripeAsync(item);
            
            var discount = _context.Discounts.FirstOrDefault(d => d.ItemID == item.ItemID);
            // Create a new price for the updated item
            var priceId = await StripeService.CreatePriceStripeAsync(item, item.StripeProductId, discount?.Percentage ?? 0);
            item.StripePriceId = priceId; // Update the Stripe Price ID in the Item entity
            item.DiscountedPrice = item.Price * (1 - (discount?.Percentage ?? 0) / 100); // Update DiscountedPrice based on the discount percentage
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
            // Deactivate the product and price in Stripe
            await StripeService.DeleteProductStripeAsync(item);

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

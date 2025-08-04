using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace FoodOrderingAPI.Repository
{
    public class ItemRepo : IItemRepo
    {
        public ApplicationDBContext _context { get; }
        string stripeKey = "put your key!!";
        public ItemRepo(ApplicationDBContext dBContext)
        {
            _context = dBContext;
        }
        // ===== Items CRUD =====
        public async Task<Item> AddItemAsync(string restaurantId, Item item)
        {
            StripeConfiguration.ApiKey = stripeKey;
            var productOptions = new ProductCreateOptions
            {
                Name = item.Name,
                Description = item.Description,
            };
            var productService = new ProductService();
            var product = await productService.CreateAsync(productOptions);
            var priceOptions = new PriceCreateOptions
            {
                UnitAmount = (long)(item.Price * 100), // Convert to cents
                Currency = "egp",
                Product = product.Id,
            };
            var priceService = new PriceService();
            var price = await priceService.CreateAsync(priceOptions);
            item.StripePriceId = price.Id; // Store the Stripe Price ID in the Item entity
            item.StripeProductId = product.Id; // Store the Stripe Product ID in the Item entity
            item.RestaurantID = restaurantId;
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            StripeConfiguration.ApiKey = stripeKey;
            // Update the product details in Stripe
            var options = new ProductUpdateOptions 
            {
                Name = item.Name,
                Description = item.Description
            };
            var service = new ProductService();
            Product product = service.Update(item.StripeProductId, options);
            
            // Deactivate the old price
            var priceOptions = new PriceUpdateOptions { Active = false };
            var priceService = new PriceService();
            Price price = priceService.Update(item.StripePriceId, priceOptions);
            
            // Create a new price for the updated item
            var newPriceOptions = new PriceCreateOptions
            {
                UnitAmount = (long)(item.Price * 100),
                Currency = "egp",
                Product = item.StripeProductId,
            };
            var newPriceService = new PriceService();
            var newPrice = await newPriceService.CreateAsync(newPriceOptions);
            item.StripePriceId = newPrice.Id; // Update the Stripe Price ID in the Item entity

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
            StripeConfiguration.ApiKey = stripeKey;
            var options = new ProductUpdateOptions { Active = false };
            var service = new ProductService();
            Product product = service.Update(item.StripeProductId, options);

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

using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class ShoppingCartRepository:IShoppingCartRepository
    {
        ApplicationDBContext dbContext;
        public ShoppingCartRepository(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<ShoppingCart> getByCustomer(string CustomerId)
        {
            return dbContext.ShoppingCarts
                ?.Include(sc => sc.ShoppingCartItems)
                ?.ThenInclude(sci =>sci.Item)
                ?.ThenInclude(item => item.Discounts)
                ?.FirstOrDefault(sh => sh.CustomerID==CustomerId);
        }
        public async Task<ShoppingCart> getById(Guid shoppingcartid)
        {
            return await dbContext.ShoppingCarts
                ?.Include(sc => sc.Customer)
                ?.Include(sc => sc.ShoppingCartItems)
                ?.ThenInclude(sci => sci.Item)
                ?.ThenInclude(item => item.Discounts)
                ?.FirstOrDefaultAsync(sh => sh.CartID == shoppingcartid);
        }
        public async Task Create(ShoppingCart cart,string customerid)
        {
            cart.CustomerID = customerid;
            await dbContext.ShoppingCarts.AddAsync(cart);
        }
        public async Task Clear(ShoppingCart cart)
        {
            cart.UpdatedAt = DateTime.Now;
            //cart.ShoppingCartItems.Clear();
            dbContext.ShoppingCartItems.RemoveRange(cart.ShoppingCartItems);

        }
    
        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

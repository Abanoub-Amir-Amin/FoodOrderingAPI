using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace FoodOrderingAPI.Repository
{
    public class ShoppingCartItemsRepository:IShoppingCartItemsRepository
    {
        ApplicationDBContext context;
        IShoppingCartRepository shoppingCartRepository;
        public ShoppingCartItemsRepository(ApplicationDBContext context, IShoppingCartRepository shoppingCartRepository)
        {
            this.context = context;
            this.shoppingCartRepository = shoppingCartRepository;
        }
        public async Task<ShoppingCartItem> getyId(Guid CatItemId)
        {
            return await context.ShoppingCartItems
                ?.Include(sci => sci.ShoppingCart)
                    ?.ThenInclude(sc => sc.Customer)
                ?.Include(sci => sci.Item)
                ?.FirstOrDefaultAsync(sci => sci.CartItemID == CatItemId);
        }
        public async Task<ShoppingCartItem> getyItemIdAndCartId(Guid ItemId,Guid CartId)
        {
            return await context.ShoppingCartItems.Include(sci => sci.Item).FirstOrDefaultAsync(sci => sci.ItemID==ItemId && sci.CartID==CartId );
        }
        public async Task addItemToShoppingCart(ShoppingCartItem shoppingCartItem)
        {
            await context.ShoppingCartItems.AddAsync(shoppingCartItem);
        }
        public async Task Update(ShoppingCartItem shoppingCartItem)
        {
            context.ShoppingCartItems.Update(shoppingCartItem);

        }
        public async Task<ShoppingCartItem> Delete(ShoppingCartItem shoppingCartItem)
        {
  
            context.ShoppingCartItems.Remove(shoppingCartItem);
            return shoppingCartItem;
        }
        public async Task Save()
        {
            await context.SaveChangesAsync();

        }

    }
}

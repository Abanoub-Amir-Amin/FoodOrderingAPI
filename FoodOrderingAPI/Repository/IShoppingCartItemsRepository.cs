using FoodOrderingAPI.Models;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public interface IShoppingCartItemsRepository
    {
        public Task<ShoppingCartItem> getyId(Guid CatItemId);
        public Task<ShoppingCartItem> getyItemIdAndCartId(Guid ItemId, Guid CartId);
        public Task addItemToShoppingCart(ShoppingCartItem shoppingCartItem);
        public Task Update(ShoppingCartItem shoppingCartItem);
        public Task<ShoppingCartItem> Delete(ShoppingCartItem ShoppingCartItem);

        public Task Save();



    }
}

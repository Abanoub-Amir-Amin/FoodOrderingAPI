using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IShoppingCartRepository
    {

        public Task<ShoppingCart> getByCustomer(string CustomerUserName);
        public Task<ShoppingCart> getById(Guid id);
        public Task Create(ShoppingCart cart, string customerid);
        public Task Clear(ShoppingCart cart);
        public Task Save();

    }
}

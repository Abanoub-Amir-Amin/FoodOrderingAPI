using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IShoppingCartServices
    {
        public Task<ShoppingCartDTO> getByCustomer(string CustomerId);
        public Task<ShoppingCartDTO> getById(Guid shoppingcartid);
        public Task<Customer> getCustomer(Guid shoppingcartid);
        public Task Create(ShoppingCart cart, string customerid);
        public Task Clear(Guid cartid);
        public Task UpdatePrices(ShoppingCart cart);

    }
}
 
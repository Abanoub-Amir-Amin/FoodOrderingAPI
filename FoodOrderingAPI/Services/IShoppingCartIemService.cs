using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;

namespace FoodOrderingAPI.Services
{
    public interface IShoppingCartIemService
    {
        public Task<ShoppingCartItemDto> getbyshoppingCartitemId(Guid shoppingcartitemid);
        public Task<ShoppingCartItemDto> getByItemIdAndCartId(Guid ItemId, Guid CartId);
        public Task<Customer> getCustomer(Guid shoppingcartItemid);
        public Task<ShoppingCartItem> AddItemToShoppingCart(ShoppingCartItemAddedDTO shoppingcartitemDto);
        public Task UpdateQuantity(Guid ItemCardId, int Addition);
        public Task Removeitem(Guid shoppingCartItemId);

    }
}

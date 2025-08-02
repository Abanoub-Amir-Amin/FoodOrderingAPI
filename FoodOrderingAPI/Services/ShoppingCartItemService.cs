using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Services
{
    public class ShoppingCartItemService:IShoppingCartIemService
    {
        IShoppingCartItemsRepository shoppingCartItemsRepository;
        IShoppingCartRepository shoppingCartRepository;
        IShoppingCartServices shoppingCartServices;
        IMapper mapper;
        ApplicationDBContext context;
        public ShoppingCartItemService(IShoppingCartItemsRepository shoppingCartItemsRepository,IShoppingCartRepository shoppingCartRepository,IShoppingCartServices shoppingCartServices, IMapper mapper, ApplicationDBContext context) 
        {
            this.shoppingCartItemsRepository = shoppingCartItemsRepository;
            this.shoppingCartRepository = shoppingCartRepository;
            this.shoppingCartServices = shoppingCartServices;
            this.mapper = mapper;
            this.context = context;
        }
        public async Task<ShoppingCartItemDto> getbyshoppingCartitemId(Guid shoppingcartitemid)
        {
            ShoppingCartItem shoppingCartItem = await shoppingCartItemsRepository.getyId(shoppingcartitemid);
            ShoppingCartItemDto shoppingCartitemDto = mapper.Map<ShoppingCartItemDto>(shoppingCartItem);
            return shoppingCartitemDto;
        }
        public async Task<ShoppingCartItemDto> getByItemIdAndCartId(Guid ItemId, Guid CartId)
        {
            ShoppingCartItem shoppingCartItem = await shoppingCartItemsRepository.getyItemIdAndCartId(ItemId,CartId);
            ShoppingCartItemDto shoppingCartitemDto = mapper.Map<ShoppingCartItemDto>(shoppingCartItem);
            return shoppingCartitemDto;
        }
        public async Task<Customer> getCustomer(Guid shoppingcartItemid)
        {
            return (await shoppingCartItemsRepository.getyId(shoppingcartItemid)).ShoppingCart.Customer;

        }

        //to check if item added before to shopping cart
        public async Task<bool> IsItemAdded(Guid itemId, Guid cartid)
        {
            return await context.ShoppingCartItems.Where(sci => (sci.ItemID == itemId && sci.CartID==cartid)).AnyAsync();
        }
        public async Task<ShoppingCartItem> AddItemToShoppingCart(ShoppingCartItemAddedDTO shoppingcartitemDto) {

            if (!await IsItemAdded(shoppingcartitemDto.ItemID,shoppingcartitemDto.CartID))
            {
                Item item = await context.Items.FindAsync(shoppingcartitemDto.ItemID);
                ShoppingCart shoppingcart=await shoppingCartRepository.getById(shoppingcartitemDto.CartID);
                if (item == null || shoppingcart == null)
                    throw new ArgumentException("Item or ShoppingCart not found.");
                if (await ValidateSameRestaurant(item, shoppingcart))
                {
                    ShoppingCartItem shoppingCartItem = new ShoppingCartItem();
                    //use item repo
                    mapper.Map(shoppingcartitemDto, shoppingCartItem);
                    shoppingCartItem.TotalPrice = item.Price;
                    await shoppingCartItemsRepository.addItemToShoppingCart(shoppingCartItem);
                    await shoppingCartServices.UpdatePrices(shoppingcart);
                    await shoppingCartItemsRepository.Save();
                    return shoppingCartItem;
                }
                else
                {
                    throw new InvalidOperationException("Cannot add item from a different restaurant.");
                }
            }
            else
            {
                ShoppingCartItem shoppingCartItem = await shoppingCartItemsRepository.getyItemIdAndCartId(shoppingcartitemDto.ItemID, shoppingcartitemDto.CartID);
                await UpdateQuantity(shoppingCartItem.CartItemID, 1);
                return shoppingCartItem;
            }
        }
        //to avoid add items from different restaurants
        public async Task<bool> ValidateSameRestaurant(Item item,ShoppingCart cart)
        {
            if(cart.RestaurantID !=null)
                return (item.RestaurantID == cart.RestaurantID);
            else
            {
                cart.RestaurantID = item.RestaurantID;
                return true;
            }
        }
        public async Task UpdateQuantity(Guid cartitemid, int Addition)
        {
            ShoppingCartItem shoppingCartItem = await shoppingCartItemsRepository.getyId(cartitemid);

            if (shoppingCartItem != null)
            {
                ShoppingCart shoppingCart = await shoppingCartRepository.getById(shoppingCartItem.CartID);
                shoppingCartItem.Quantity += Addition;
                shoppingCartItem.TotalPrice = shoppingCartItem.Quantity * shoppingCartItem.Item.Price;
                await shoppingCartServices.UpdatePrices(shoppingCart);
                await shoppingCartItemsRepository.Save();


            }
            else
                throw new ArgumentException("Item not found in this shopping cart.");


        }
        public async Task Removeitem(Guid shoppingCartItemId)
        {
            ShoppingCartItem shoppingCartItem = await shoppingCartItemsRepository.getyId(shoppingCartItemId);
            if (shoppingCartItem == null)
                throw new ArgumentNullException("this item removed");
            await shoppingCartItemsRepository.Delete(shoppingCartItem);
            ShoppingCart shoppingCart = await shoppingCartRepository.getById(shoppingCartItem.CartID);
            await shoppingCartServices.UpdatePrices(shoppingCart);
            await shoppingCartItemsRepository.Save();

        }


    }
    }

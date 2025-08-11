using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Services
{
    public class ShoppingCartServices:IShoppingCartServices
    {
        IShoppingCartRepository shoppingCartRepository;
        ICustomerRepo customerRepo;
        IMapper mapper;
        public ShoppingCartServices(IShoppingCartRepository shoppingCartRepository, ICustomerRepo customerRepo,IMapper mapper) 
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this .customerRepo = customerRepo;
            this.mapper = mapper;
        }
        public async Task<ShoppingCartDTO> getByCustomer(string CustomerId)
        {

             if(await customerRepo.GetById(CustomerId)==null)
            {
                throw new ArgumentNullException($"this Customer Id {CustomerId} not found");
            }
            else
            {
                ShoppingCart cart= await shoppingCartRepository.getByCustomer(CustomerId);
                ShoppingCartDTO shoppingCartDTO = mapper.Map<ShoppingCartDTO>(cart);
                return shoppingCartDTO;
            }

        }
        public async Task<ShoppingCartDTO> getById(Guid shoppingcartid)
        {
            ShoppingCart cart = await shoppingCartRepository.getById(shoppingcartid);
            if (cart==null)
            {
                throw new ArgumentNullException($"this shoppingcart Id {shoppingcartid} not found");
            }
            else
            {
                ShoppingCartDTO shoppingCartDTO = mapper.Map<ShoppingCartDTO>(cart);
                return shoppingCartDTO;
            }
        }
        public async Task<Customer> getCustomer(Guid shoppingcartid) {
            return (await shoppingCartRepository.getById(shoppingcartid)).Customer;
        }
        public async Task Create(ShoppingCart cart, string customerid)
        {
           await shoppingCartRepository.Create(cart, customerid);
           await  shoppingCartRepository.Save();
        }
        public async Task Clear(Guid cartid)
        {
            ShoppingCart cart = await shoppingCartRepository.getById(cartid);

            await shoppingCartRepository.Clear(cart);
            await shoppingCartRepository.Save();
            await UpdatePrices(cart);

        }
        
        public async Task UpdatePrices(ShoppingCart cart)
        {
            if (cart.ShoppingCartItems == null || !cart.ShoppingCartItems.Any())
            {
                cart.SubTotal = 0;
            }
            else
            {
                cart.SubTotal = cart.ShoppingCartItems.Sum(sci => sci.TotalPrice);
            }

            await shoppingCartRepository.Save();
        }

    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        public IShoppingCartServices ShoppingCartServices;
        public IShoppingCartIemService shoppingCartIemService;
        public ShoppingCartController(IShoppingCartServices ShoppingCartServices, IShoppingCartIemService shoppingCartIemService)
        {
            this.ShoppingCartServices = ShoppingCartServices;
            this.shoppingCartIemService = shoppingCartIemService;
        }
        [HttpGet("ShoppingCart")]
        public async Task<IActionResult> Get(string customerid)
        {
       
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != customerid)
            {
                return Forbid("You are not authorized to View this Customer's ShoppingCart.");
            }
            try
            {
                var cart = await ShoppingCartServices.getByCustomer(customerid);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving shopping cart: {ex.Message}");
            }
        }

        [HttpPut("Clear")]
        public async Task<IActionResult> Clear(Guid cartid)
        {
            Customer Customer = await ShoppingCartServices.getCustomer(cartid);
            if (Customer == null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != Customer.CustomerID)
            {
                return Forbid("You are not authorized to Clear this Customer's ShoppingCart.");
            }
            try
            {
                await ShoppingCartServices.Clear(cartid);
                return Ok("shopping cart cleared successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error clearing cart: {ex.Message}");
            }
        }

        [HttpPost("addItem")]
        public async Task<IActionResult> addItem(ShoppingCartItemAddedDTO item)
        {
            Customer Customer = await ShoppingCartServices.getCustomer(item.CartID);
            if (Customer == null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != Customer.CustomerID)
            {
                return Forbid("You are not authorized to Add item to this Customer's ShoppingCart.");
            }
            try
            {
                ShoppingCartItem cartitem = await shoppingCartIemService.AddItemToShoppingCart(item);
                return Created(string.Empty, new
                {
                    message = "Item added to shopping cart successfully",
                    cartItem = cartitem
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // item or cart not found
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // different restaurant
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding item: {ex.Message}");
            }
        }

        [HttpPut("UpdateItem")]
        public async Task<IActionResult> UpdateItem(Guid cartIemId, int addition)
        {
            Customer Customer = await shoppingCartIemService.getCustomer(cartIemId);
            if (Customer == null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != Customer.CustomerID)
            {
                return Forbid("You are not authorized to Update item from this Customer's ShoppingCart."+userIdClaim+" , "+Customer.CustomerID);
            }
            try
            {
                await shoppingCartIemService.UpdateQuantity(cartIemId, addition);
                return Ok("item updated to shopping cart successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating item: {ex.Message}");
            }
        }

        [HttpDelete("RemoveItem")]
        public async Task<IActionResult> RemoveItem(Guid cartIemId)
        {
            Customer Customer = await shoppingCartIemService.getCustomer(cartIemId);
            if (Customer == null)
                return NotFound();
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != Customer.CustomerID)
            {
                return Forbid("You are not authorized to Delete item from this Customer's ShoppingCart.");
            }
            try
            {
                await shoppingCartIemService.Removeitem(cartIemId);
                return Ok("item removed from shopping cart successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest( $"Error removing item: {ex.Message}");
            }
        }
    }
}
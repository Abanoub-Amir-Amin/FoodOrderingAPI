using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Controllers
{
    [EnableCors("AllowAngularDevClient")]
    [Authorize(Roles = "Restaurant")]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IItemService _ItemService;
        private readonly IRestaurantService _RestaurantService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ItemController(IItemService service, IRestaurantService restaurantService, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _ItemService = service;
            _RestaurantService = restaurantService;
            _context = context;
            _mapper = mapper;
            _environment = environment;

        }
        // ===== Items CRUD =====
        [Consumes("multipart/form-data")]  // Ensure it accepts multipart/form-data
        [HttpPost("{restaurantId}/items")]
        public async Task<IActionResult> AddItem(string restaurantId, [FromForm] ItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var item = await _ItemService.AddItemAsync(restaurantId, dto);
            await _ItemService.CreateItemAsync(restaurantId, dto); ///RecieveItem event must be subscribed by Angular to get latest items in real time in addition to GetItem end point.
            return CreatedAtAction(nameof(GetItem), new { restaurantId, itemId = item.ItemID }, item);
        }


        [HttpGet("items/{itemId}")]
        public async Task<IActionResult> GetItem(Guid itemId)
        {
           
            var item = await _ItemService.GetItemByIdAsync(itemId);

            if (item == null)
                return NotFound($"There are no such item with ID '{itemId}'");

            return Ok(item);
        }


        [HttpPut("items/{itemId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateItem(Guid itemId, [FromForm] ItemDto dto)
        {
            
            var item = await _ItemService.UpdateItemAsync(itemId, dto);

            if (item == null)
                return NotFound($"Item with ID '{itemId}' not found.");

            return Ok(item);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(Guid itemId)
        {
            var success = await _ItemService.DeleteItemAsync(itemId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("items/bycategory")]
        public async Task<IActionResult> GetItemsByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category must be provided.");
            var items = await _ItemService.GetItemsByCategoryAsync(category);
            return Ok(items);
        }
        [AllowAnonymous]
        [HttpGet("items/byrestaurantname")]
        public async Task<IActionResult> GetItemsByRestaurantName([FromQuery] string restaurantName)
        {
            if (string.IsNullOrWhiteSpace(restaurantName))
                return BadRequest("Restaurant Name must be provided.");
            var items = await _ItemService.GetItemsByRestaurantAsync(restaurantName);
            return Ok(items);
        }

        [HttpGet("items")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await _ItemService.GetAllItemsAsync());
        }

        [HttpGet("{restaurantId}/items/most-ordered")]
        public async Task<IActionResult> GetMostOrderedItems(string restaurantId)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var items = await _ItemService.GetMostOrderedItemsAsync(restaurantId);

            return Ok(items);
        }
        [HttpGet("items/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _ItemService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound("No categories found.");
            return Ok(categories);
        }
    }
}

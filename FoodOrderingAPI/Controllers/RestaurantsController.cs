using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Controllers
{
    [Authorize(Roles = "Restaurant")]
    [ApiController]
    [Route("api/restaurants")]
    public class RestaurantController : ControllerBase
    {
        private readonly ApplicationDBContext _context;  
        private readonly IRestaurantService _service;    
        private readonly IMapper _mapper;                 

        
        public RestaurantController(IRestaurantService service, ApplicationDBContext context, IMapper mapper)
        {
            _service = service;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("{restaurantId}/items")]
        public async Task<IActionResult> AddItem(string restaurantId, [FromBody] ItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.RestaurantID == restaurantId && r.User.Role == RoleEnum.Restaurant);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var item = await _service.AddItemAsync(restaurantId, dto);

            return CreatedAtAction(nameof(GetItem), new { restaurantId, itemId = item.ItemID }, item);
        }


        [HttpGet("{restaurantId}/items/{itemId}")]
        public async Task<IActionResult> GetItem(string restaurantId, Guid itemId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId == restaurantId && r.User.Role == RoleEnum.Restaurant);

            var item = await _service.GetItemByIdAsync(restaurantId, itemId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            if (item == null)
                return NotFound($"There are no such item with ID '{itemId}'");

            return Ok(item);
        }


        //[HttpPut("{restaurantId}/items/{itemId}")]
        //public async Task<IActionResult> UpdateItem(string restaurantId, Guid itemId, [FromBody] ItemDto dto)
        //{
        //    var restaurant = await _context.Restaurants
        //        .FirstOrDefaultAsync(r => r.UserId == restaurantId && r.User.Role == RoleEnum.Restaurant);

        //    if (restaurant == null)
        //        return NotFound($"Restaurant with ID '{restaurantId}' not found.");

        //    if (!restaurant.IsActive)
        //        return Forbid("Your restaurant account is not yet active.");


        //    var item = await _service.UpdateItemAsync(restaurantId, dto);

        //    if (item == null)
        //        return NotFound($"Item with ID '{itemId}' not found.");

        //    return Ok(item);
        //}

       
        [HttpDelete("{restaurantId}/items/{itemId}")]
        public async Task<IActionResult> DeleteItem(string restaurantId, Guid itemId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.UserId == restaurantId && r.User.Role == RoleEnum.Restaurant);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var success = await _service.DeleteItemAsync(itemId, restaurantId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{restaurantId}/items/bycategory")]
        public async Task<IActionResult> GetItemsByCategory(string restaurantId, [FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category must be provided.");

            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null || !restaurant.IsActive)
                return NotFound("Restaurant not found or inactive.");

            var items = await _service.GetItemsByCategoryAsync(restaurantId, category);
            return Ok(items);
        }

        [HttpPost("{restaurantId}/discounts")]
        public async Task<IActionResult> AddDiscount(string restaurantId, [FromBody] DiscountDto dto)
        {
            var discount = new Discount
            {
                ItemID = dto.ItemID,
                Percentage = dto.Percentage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            var result = await _service.AddDiscountAsync(restaurantId, discount);

            return Ok(result);
        }

        [HttpPut("{restaurantId}/discounts/{discountId}")]
        public async Task<IActionResult> UpdateDiscount(string restaurantId, int discountId, [FromBody] DiscountDto dto)
        {
            var discount = _mapper.Map<Discount>(dto);
            discount.DiscountID = discountId;
            discount.RestaurantID = restaurantId;

            var updatedDiscount = await _service.UpdateDiscountAsync(discount);
            return Ok(updatedDiscount);
        }

        [HttpDelete("{restaurantId}/discounts/{discountId}")]
        public async Task<IActionResult> DeleteDiscount(string restaurantId, int discountId)
        {
            var success = await _service.DeleteDiscountAsync(discountId, restaurantId);
            if (!success) return NotFound();
            return NoContent();
        }


        [HttpPost("{restaurantId}/promocodes")]
        public async Task<IActionResult> AddPromoCode(string restaurantId, [FromBody] PromoCodeDto dto)
        {
            var promoCode = new PromoCode
            {
                Code = dto.Code,
                DiscountPercentage = dto.DiscountPercentage,
                IsFreeDelivery = dto.IsFreeDelivery,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit
            };

            var result = await _service.AddPromoCodeAsync(restaurantId, promoCode);

            return Ok(result);
        }

        [HttpPut("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> UpdatePromoCode(string restaurantId, Guid promoCodeId, [FromBody] PromoCodeDto dto)
        {
            var promoCode = _mapper.Map<PromoCode>(dto);
            promoCode.PromoCodeID = promoCodeId;
            promoCode.RestaurantID = restaurantId;

            var updatedPromoCode = await _service.UpdatePromoCodeAsync(promoCode);
            return Ok(updatedPromoCode);
        }

        [HttpDelete("{restaurantId}/promocodes/{promoCodeId}")]
        public async Task<IActionResult> DeletePromoCode(string restaurantId, Guid promoCodeId)
        {
            var success = await _service.DeletePromoCodeAsync(promoCodeId, restaurantId);
            if (!success) return NotFound();
            return NoContent();
        }


        [HttpPut("{restaurantId}/orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string restaurantId, Guid orderId, [FromBody] OrderStatusUpdateDto dto)
        {
            // Authentication/Authorization: Ensure the restaurant ID from the route matches the authenticated user's restaurant ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != restaurantId)
            {
                return Forbid("You are not authorized to update orders for this restaurant.");
            }
            

            try
            {
                var order = await _service.UpdateOrderStatusAsync(orderId, dto.Status, restaurantId);

                if (order == null)
                    return NotFound();

                return Ok(order);
            }
            catch (InvalidOperationException ex) // Catches the IsAvailable check from service
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating order status: " + ex.Message });
            }
        }

        [HttpPost("apply")]
        [AllowAnonymous]
        public async Task<IActionResult> ApplyToJoin([FromBody] RestaurantDto dto)
        {
            try
            {
                // Call service to apply and create restaurant user
                var result = await _service.ApplyToJoinAsync(dto);

                // Return 201 Created with route to newly created resource
                return CreatedAtAction(nameof(GetRestaurantById), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                // Duplicate user detected - return client-friendly 400 Bad Request
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Input validation error
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Unexpected error - return 500 Internal Server Error with message
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestaurantById(string id)
        {
            try
            {
                var restaurant = await _service.GetRestaurantByIdAsync(id);

                if (restaurant == null)
                    return NotFound();

                var dto = _mapper.Map<RestaurantDto>(restaurant);
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("update profile")]
        public async Task<IActionResult> UpdateRestaurantProfile(string restaurantId, [FromBody] RestaurantProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Authorizing that the authenticated user is the owner of this restaurant ID
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != restaurantId)
            {
                return Forbid("You are not authorized to update this restaurant's profile.");
            }

            try
            {
                var updatedRestaurant = await _service.UpdateRestaurantProfileAsync(restaurantId, dto);

                if (updatedRestaurant == null)
                {
                    return NotFound($"Restaurant with ID '{restaurantId}' not found.");
                }

                // Map the updated entity back to a DTO for the response
                var responseDto = _mapper.Map<RestaurantDto>(updatedRestaurant);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the restaurant profile: " + ex.Message });
            }
        }

        [HttpGet("api/restaurants/{restaurantId}/orders")]
        public async Task<IActionResult> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            var restaurant = await _service.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var orders = await _service.GetAllOrdersByRestaurantAsync(restaurantId);

            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(ordersDto);
        }

        [HttpGet("{restaurantId}/orders/status")]
        public async Task<IActionResult> GetOrdersByStatus(string restaurantId, [FromQuery] string status)
        {
            var restaurant = await _service.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var allowedStatuses = new[] {  "Preparing", "Out for Delivery", "Delivered", "Cancelled" };
            string[] requestedStatuses;

            if (string.IsNullOrWhiteSpace(status))
            {
                // If no status param provided, return all allowed
                requestedStatuses = allowedStatuses;
            }
            else
            {
                requestedStatuses = status.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => allowedStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                if (requestedStatuses.Length == 0)
                    return BadRequest("No valid order statuses provided for filtering.");
            }

            var ordersDto = await _service.GetOrdersByStatusAsync(restaurantId, requestedStatuses);

            return Ok(ordersDto);
        }

        [HttpGet("{restaurantId}/dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary(string restaurantId)
        {
            var restaurant = await _service.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var summary = await _service.GetDashboardSummaryAsync(restaurantId);

            return Ok(summary);
        }

        [HttpGet("{restaurantId}/items/most-ordered")]
        public async Task<IActionResult> GetMostOrderedItems(string restaurantId)
        {
            var restaurant = await _service.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var items = await _service.GetMostOrderedItemsAsync(restaurantId);

            return Ok(items);
        }

    }
}

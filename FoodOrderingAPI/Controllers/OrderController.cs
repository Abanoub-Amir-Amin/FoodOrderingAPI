using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOrderService _OrderService;
        private readonly IRestaurantService _RestaurantService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public OrderController(IOrderService service, IRestaurantService restaurantService, ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _OrderService = service;
            _RestaurantService = restaurantService;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;

        }
        // ===== Orders =====
        [HttpGet("{restaurantId}/orders")]
        public async Task<IActionResult> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var orders = await _OrderService.GetAllOrdersByRestaurantAsync(restaurantId);

            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(ordersDto);
        }

        [HttpGet("{restaurantId}/orders/status")]
        public async Task<IActionResult> GetOrdersByStatus(string restaurantId, [FromQuery] string status)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var allowedStatuses = new[] { "Preparing", "Out_for_Delivery", "Delivered"};
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

            var ordersDto = await _OrderService.GetOrdersByStatusAsync(restaurantId, requestedStatuses);

            return Ok(ordersDto);
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
                var order = await _OrderService.UpdateOrderStatusAsync(orderId, dto.Status, restaurantId);

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


        // ===== Dashboard Summary =====
        [HttpGet("{restaurantId}/dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary(string restaurantId)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var summary = await _OrderService.GetDashboardSummaryAsync(restaurantId);

            return Ok(summary);
        }
    }
}

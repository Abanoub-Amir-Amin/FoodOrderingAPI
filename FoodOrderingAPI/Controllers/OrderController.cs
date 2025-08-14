using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public OrderController(IOrderService service, IRestaurantService restaurantService, IShoppingCartRepository shoppingCartRepository,
            ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _OrderService = service;
            _RestaurantService = restaurantService;
            _shoppingCartRepository = shoppingCartRepository;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;

        }
        // ===== Orders =====
        //[Authorize(Roles = "Restaurant")]

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
        [Authorize(Roles = "Restaurant")]

        [HttpGet("{restaurantId}/orders/status")]
        //[Authorize(Roles = "Restaurant")]

        public async Task<IActionResult> GetOrdersByStatus(string restaurantId, [FromQuery] StatusEnum[] status)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            //var allowedStatuses = new[] { StatusEnum.Preparing, StatusEnum.Out_for_Delivery, StatusEnum.Cancelled };
            var allowedStatuses = new[] { StatusEnum.Preparing, StatusEnum.Out_for_Delivery };
            StatusEnum[] requestedStatuses;

            if (!status.Any())
            {
                // If no status param provided, return all allowed
                requestedStatuses = allowedStatuses;
            }
            else
            {
                requestedStatuses = status
                    .Where(s => allowedStatuses.Contains(s))
                    .ToArray();
                if (requestedStatuses.Length == 0)
                    return BadRequest("No valid order statuses provided for filtering.");
            }

            var ordersDto = await _OrderService.GetOrdersByStatusAsync(restaurantId, requestedStatuses);

            return Ok(ordersDto);
        }

        [HttpPut("{restaurantId}/orders/{orderId}/status")]
        //[Authorize(Roles = "Restaurant")]

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
        //[Authorize(Roles = "Restaurant")]
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
        //[HttpPut("CancelOrder")]
        //public async Task<IActionResult> CancelOrder(Guid OrderId, [FromBody] string reson)
        //{
        //    //check orderid exist
        //    Order order = await _OrderService.getOrder(OrderId);
        //    if (order == null) return NotFound("this orderid dosen't meet any order");

        //    //check authersity of restaurant
        //    var restaurantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (restaurantId != order.RestaurantID)
        //        return Unauthorized($"this user with userId{restaurantId} not autherized to cancel this orderId");

        //    bool cancelled = await _OrderService.CancelOrder(order, reson);
        //    if (cancelled)
        //        return Ok("Order Cancelled Sucessfully");
        //    return BadRequest("order Status not correct");
        //}
        [HttpPut("ConfirmOrder")]
        //[Authorize(Roles = "Restaurant")]

        public async Task<IActionResult> ConfirmOrder(Guid OrderId)
        {
            Order order = await _OrderService.getOrder(OrderId);
            if (order == null) return NotFound("this orderid dosen't meet any order");

            //check authersity of restaurant
            var restaurantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (restaurantId != order.RestaurantID)
                return Unauthorized($"this user with userId{restaurantId} not autherized to confirm this orderId");

            bool Assigned = await _OrderService.assignDelivaryManToOrder(order);
            if (!Assigned)
                return BadRequest("there are not available DelivaryMen Now");

            bool confirmed = await _OrderService.ConfirmOrder(order);

            if (confirmed)
                return Ok("Order confirmed Sucessfully");
            return BadRequest("order Status not correct");

        }

        [Authorize(Roles = "Customer")]
        [HttpGet("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart cart = await _shoppingCartRepository.getByCustomer(CustomerId);
            if (cart == null) return NotFound("customer or shopping car Not found");

            CheckoutViewDTO checkout = await _OrderService.Checkout(cart);
            return Ok(checkout);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder()
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart cart = await _shoppingCartRepository.getByCustomer(CustomerId);
            if (cart == null) return NotFound("customer or shopping car Not found");
            try
            {
                await _OrderService.PlaceOrder(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("order Placed Successd");
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("AllOrdersForCustomer")]
        public async Task<IActionResult> GetOrdersForCustomer()
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.getOrders(CustomerId);
            return Ok(orders);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("OrderDetailaForCustomer")]
        public async Task<IActionResult> GetOrderDetailsForCustomer(Guid orderId)
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _OrderService.getOrder(orderId);
            if (order.CustomerID!=CustomerId)
                return Unauthorized($"this user with userId{CustomerId} not autherized to view this orderId");

            var orderDetails =await _OrderService.getOrderDetails(orderId);
            if (orderDetails == null) return NotFound();
            return Ok(orderDetails);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("OrderForCustomerbystatus")]
        public async Task<IActionResult> GetOrderForCustomerByStatus(StatusEnum[] status)
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.GetOrdersByStatusAsyncForCustomer(CustomerId, status);
            return Ok(orders);
        }

        //---------------DelivaryMan-------------

        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("PreparingOrdersForDelivary")]
        public async Task<IActionResult> GetOrdersForDelivary()
        {
            var DelivaryId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.getOrdersForDelivarMan(DelivaryId);
            return Ok(orders);
        }
    }
}
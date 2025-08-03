using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public class OrderService:IOrderService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDBContext _context;
        private readonly IOrderRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public OrderService(IOrderRepo repository, ApplicationDBContext context, IMapper mapper, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        // Orders
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _repository.GetAllOrdersByRestaurantAsync(restaurantId);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, string[] statuses)
        {
            var orders = await _repository.GetAllOrdersByRestaurantAsync(restaurantId);

            // Filter by status using case-insensitive comparison
            var filteredOrders = orders.Where(o => statuses.Contains(o.Status, StringComparer.OrdinalIgnoreCase));

            return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders);
        }
        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId)
        {
            var allowedStatuses = new[] { "Preparing", "Out for Delivery", "Canceled" };
            if (!allowedStatuses.Contains(status))
                throw new ArgumentException("Invalid order status.");

            return await _repository.UpdateOrderStatusAsync(orderId, status, restaurantId);
        }

        //Dashboard Summary
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            return await _repository.GetDashboardSummaryAsync(restaurantId);
        }
    }
}

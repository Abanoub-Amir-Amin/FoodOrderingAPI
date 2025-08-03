using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class OrderRepo:IOrderRepo
    {
        private readonly ApplicationDBContext _context;

        public OrderRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        // ===== Orders =====
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _context.Orders
                .Where(o => o.RestaurantID == restaurantId)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderID == orderId && o.RestaurantID == restaurantId);
            if (order == null)
                return null;

            order.Status = status;
            await _context.SaveChangesAsync();
            return order;
        }

        // ===== Dashboard Summary =====
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            var orders = await GetAllOrdersByRestaurantAsync(restaurantId);

            var deliveredCount = orders.Count(o => o.Status == "Delivered");
            var inProcessCount = orders.Count(o => o.Status == "Preparing" || o.Status == "Out for Delivery");
            var cancelledCount = orders.Count(o => o.Status == "Cancelled");

            return new DashboardSummaryDto
            {
                DeliveredOrders = deliveredCount,
                InProcessOrders = inProcessCount,
                CancelledOrders = cancelledCount
            };
        }
    }
}

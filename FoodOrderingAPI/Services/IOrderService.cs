using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IOrderService
    {
        //Order-CRUD
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, string[] statuses);

        Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId);

        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);
    }
}

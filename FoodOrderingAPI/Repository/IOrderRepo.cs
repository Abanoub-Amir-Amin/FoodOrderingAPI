using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IOrderRepo
    {
        //Order
        Task<Order> UpdateOrderStatusAsync(Guid orderId, string status, string restaurantId);
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);

        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);
    }
}

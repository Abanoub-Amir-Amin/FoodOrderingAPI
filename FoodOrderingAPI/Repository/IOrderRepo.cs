using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderingAPI.Repository
{
    public interface IOrderRepo
    {
        //Order
        Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum status, string restaurantId);
        Task<IEnumerable<OrderDto>> GetAllOrdersByRestaurantAsync(string restaurantId);

        public Task CancelOrder(Order order);
        public Task ConfirmOrder(Order order);
        public Task AssignOrderToDelivaryMan(Order order, string DelivaryId);


        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);

        //customer
        Task<int> GenerateOrderNumberAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task AddOrder(Order order);
        Task AddOrderItem(OrderItem orderitem);
        Task saveChangesAsync();


        Task<List<Order>> getOrders(string customerId);
        Task<List<Order>> getOrdersDelivaryMan(string DelivaryId, StatusEnum status);

        Task<Order?> GetOrderDetails(Guid orderId);

     


    }
}

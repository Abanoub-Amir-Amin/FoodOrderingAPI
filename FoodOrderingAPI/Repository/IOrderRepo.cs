using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderingAPI.Repository
{
    public interface IOrderRepo
    {
        //Order
        Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum status, string restaurantId);
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);

        public Task CancelOrder(Order order);
        public Task ConfirmOrder(Order order);
        public Task AssignOrderToDelivaryMan(Order order, string DelivaryId);


        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);

        //customer
        public Task<int> GenerateOrderNumberAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task AddOrder(Order order);
        public Task AddOrderItem(OrderItem orderitem);
        public Task saveChangesAsync();


        public Task<List<Order>> getOrders(string customerId);
        public Task<Order?> getOrderDetails(Guid orderId);




    }
}

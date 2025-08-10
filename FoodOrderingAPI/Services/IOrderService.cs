using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IOrderService
    {
        //Order-CRUD
        Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, StatusEnum[] statuses);

        Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum status, string restaurantId);

        //Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);

        //operation of restaurant to order
        public Task<bool> CancelOrder(Order order,string reason);
        public Task<bool> ConfirmOrder(Order order);
        public Task<bool> assignDelivaryManToOrder(Order order);

        //customer
        public Task<CheckoutViewDTO> Checkout(ShoppingCart shoppingCart);
        public Task PlaceOrder(NewOrderDTO orderdto, ShoppingCart cart);
        public Task<List<OrderViewDTO>> getOrders(string customerId);
        public Task<List<OrderViewDTO>> GetOrdersByStatusAsyncForCustomer(string customerId, StatusEnum[] statuses);
        public Task<OrderDetailDTO?> getOrderDetails(Guid orderId);
        public Task<Order?> getOrder(Guid orderId);
        public Task<List<DelivaryOrderDTO>> getOrdersForDelivarMan(string DelivaryId);

    }
}

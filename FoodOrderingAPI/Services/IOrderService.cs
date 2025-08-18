using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Services
{
    public interface IOrderService
    {
        // Order-CRUD
        Task<IEnumerable<OrderDto>> GetAllOrdersByRestaurantAsync(string restaurantId);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, StatusEnum status);
        Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum status, string restaurantId);

        // Dashboard Summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId);

        // Operations on orders by restaurant
        Task<(bool Success, string Message)> ConfirmOrder(Order order);
        Task<bool> assignDelivaryManToOrder(Order order);
        Task<(bool Success, string Message)> assignDelivaryManToOrderDetailed(Order order);

        // Customer operations
        Task<CheckoutViewDTO> Checkout(ShoppingCart shoppingCart);
        Task PlaceOrder(ShoppingCart cart);
        Task<List<OrderViewDTO>> getOrders(string customerId);
        Task<List<OrderViewDTO>> GetOrdersByStatusAsyncForCustomer(string customerId, StatusEnum[] statuses);
        Task<Order?> getOrder(Guid orderId);
        Task<List<DelivaryOrderDTO>> getOrdersForDelivarMan(string DelivaryId);
    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrderingAPI.Repository
{
    public class OrderRepo:IOrderRepo
    {
        private readonly ApplicationDBContext _context;
        INotificationRepo notificationrepo;
        public OrderRepo(ApplicationDBContext context, INotificationRepo notificationrepo)
        {
            _context = context;
            this.notificationrepo = notificationrepo;
        }
        // ===== Orders =====
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _context.Orders
                .Where(o => o.RestaurantID == restaurantId)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum status, string restaurantId)
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

            var deliveredCount = orders.Count(o => o.Status == StatusEnum.Delivered);
            var inProcessCount = orders.Count(o => o.Status == StatusEnum.Preparing || o.Status == StatusEnum.Out_for_Delivery);
            //var cancelledCount = orders.Count(o => o.Status == StatusEnum.Cancelled);

            return new DashboardSummaryDto
            {
                DeliveredOrders = deliveredCount,
                InProcessOrders = inProcessCount,
                //CancelledOrders = cancelledCount
            };
        }
        //========operation of order by restaurant==========
        public async Task CancelOrder(Order order)
        {
                //order.Status = StatusEnum.Cancelled;
                await _context.SaveChangesAsync();
        }
        public async Task ConfirmOrder(Order order)
        {
                order.Status = StatusEnum.Preparing;
                //can add time of order in stage of confirm instead add as field in restaurant
                //order.OrderTimeToComplete = orderTime;
                await _context.SaveChangesAsync();
        }

        public async Task AssignOrderToDelivaryMan(Order order,string DelivaryId)
        {
            order.DeliveryManID = DelivaryId;
            await _context.SaveChangesAsync();
        }

        //========Order For Customer=========
        public async Task<int> GenerateOrderNumberAsync()
        {
            var lastOrder = await _context.Orders
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            return (lastOrder?.OrderNumber ?? 10000) + 1;
        }
        //place order functions
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task AddOrder(Order order)
        {
            order.OrderNumber = await GenerateOrderNumberAsync();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrderItem(OrderItem orderitem)
        {
            await _context.OrderItems.AddAsync(orderitem);
            await _context.SaveChangesAsync();
        }
        public async Task saveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //get orders for customer
        public async Task<List<Order>> getOrders(string customerId)
        {
            return await _context.Orders
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                //.Include(o => o.PaymentTransactions)
                .Where(o => o.CustomerID==customerId)
                .ToListAsync();

        }
        public async Task<List<Order>> getOrdersDelivaryMan(string DelivaryId)
        {
            return await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Restaurant)
                .ThenInclude(R => R.User)
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .ThenInclude(C => C.User)
                .Include(c => c.OrderItems)
                .ThenInclude(OI => OI.Item)
                //.Include(o => o.PaymentTransactions)
                .Where(o => o.DeliveryManID == DelivaryId && o.Status== StatusEnum.Preparing)
                .ToListAsync();

        }
        public async Task<Order?> getOrderDetails(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.Restaurant)
                .ThenInclude(R => R.User)
                .Include(o => o.OrderItems)
                //.Include(o => o.PaymentTransactions)
                .Include(o => o.Address)
                .Include(o => o.DeliveryMan)
                .ThenInclude(D => D.User)
                .Include(o => o.PromoCode)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }
        

    }
}

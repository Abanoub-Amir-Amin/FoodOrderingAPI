using AutoMapper;

using FoodOrderingAPI.DTO;

using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;

using FoodOrderingAPI.Interfaces;

using FoodOrderingAPI.Models;

using FoodOrderingAPI.Repository;

using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Identity;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;

namespace FoodOrderingAPI.Services

{

    public class OrderService : IOrderService

    {

        private readonly IWebHostEnvironment _environment;

        private readonly IStripeService stripeService;

        private readonly ApplicationDBContext _context;

        private readonly IOrderRepo _repository;

        private readonly INotificationRepo _notificationRepo;

        private readonly IDeliveryManService _deliveryManService;

        private readonly IAddressRepo _addressRepo;

        private readonly IPromoCodeService _promoCodeService;

        private readonly IOpenRouteService _openRouteService;

        private readonly IShoppingCartServices _shoppingCartService;

        private readonly IMapper _mapper;

        private readonly UserManager<User> _userManager;


        public OrderService(

            IOrderRepo repository,

            INotificationRepo notificationRepo,

            IDeliveryManService deliveryManService,

            IAddressRepo addressRepo,

            IOpenRouteService openRouteService,

            IPromoCodeService promoCodeService,

            IShoppingCartServices shoppingCartService,

            ApplicationDBContext context,

            IMapper mapper,

            UserManager<User> userManager,

            IWebHostEnvironment environment,

            IStripeService stripeService

            )

        {

            _repository = repository;

            _notificationRepo = notificationRepo;

            _deliveryManService = deliveryManService;

            _addressRepo = addressRepo;

            _openRouteService = openRouteService;

            _promoCodeService = promoCodeService;

            _shoppingCartService = shoppingCartService;

            _context = context;

            _mapper = mapper;

            _userManager = userManager;

            _environment = environment ?? throw new ArgumentNullException(nameof(environment));

            this.stripeService = stripeService;


        }

        // Orders

        public async Task<IEnumerable<OrderDto>> GetAllOrdersByRestaurantAsync(string restaurantId)

        {

            return await _repository.GetAllOrdersByRestaurantAsync(restaurantId);

        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, StatusEnum status)

        {

            var orders = await _repository.GetAllOrdersByRestaurantAsync(restaurantId);

            var filteredOrders = orders.Where(o => o.Status == status);

            return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders);

        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum newStatus, string restaurantId)
        {
            var allowedStatuses = new[]
            {
                StatusEnum.Preparing,
                StatusEnum.Out_for_Delivery,
                StatusEnum.Cancelled,
                StatusEnum.WaitingToConfirm,
                StatusEnum.Delivered
            };

            if (!allowedStatuses.Contains(newStatus))
                throw new ArgumentException("Invalid order status.");

            var order = await _repository.GetOrderDetails(orderId); // returns entity
            if (order == null)
                throw new ArgumentException("Order not found.");

            var oldStatus = order.Status;

            if (oldStatus == StatusEnum.WaitingToConfirm && newStatus == StatusEnum.Preparing)
            {
                //order.Status = StatusEnum.Preparing;
                bool assigned = await assignDelivaryManToOrder(order);

                if (!assigned)
                    throw new InvalidOperationException("No delivery man available to assign to this order.");

                var confirmResult = await ConfirmOrder(order);
                if (!confirmResult.Success)
                    throw new InvalidOperationException("Order confirmation failed.");

            
                await _repository.saveChangesAsync();
            }
            else if (oldStatus == StatusEnum.WaitingToConfirm && newStatus == StatusEnum.Cancelled)
            {
                bool cancelled = await CancelOrder(order, "Cancelled by restaurant");
                if (!cancelled)
                    throw new InvalidOperationException("Order cancellation failed.");
                order.Status = StatusEnum.Cancelled; // make sure to set the status here if changed in CancelOrder
            }
            else
            {
                order.Status = newStatus;
                await _repository.saveChangesAsync();
            }

            if (newStatus == StatusEnum.Out_for_Delivery)
            {
                _notificationRepo.CreateNotificationTo(order.CustomerID,
                    $"Order number {order.OrderNumber} is out for delivery");
            }

            return order;
        }

        // if (order.RestaurantID != restaurantId)
        //    throw new UnauthorizedAccessException("This restaurant does not own the order.");

        // Cancel order from restaurant

        public async Task<bool> CancelOrder(Order order, string reason)

        {

            if (order == null || order.Status != StatusEnum.WaitingToConfirm)

                return false;

            await _repository.CancelOrder(order);

            // Return payment logic (commented out in original code)

            /*

            var options = new RefundCreateOptions

            {

                PaymentIntent = order.PaymentIntentId,

            };

            var service = new RefundService();

            Refund refund = await service.CreateAsync(options);

            */

            _notificationRepo.CreateNotificationTo(order.CustomerID,

                $"Order number {order.OrderNumber} was cancelled by restaurant.,\n Reason: {reason}");

            return true;

        }

        // Confirm order

        public async Task<(bool Success, string Message)> ConfirmOrder(Order order)

        {

            if (order == null)

                return (false, "Order cannot be null.");

            if (order.Status != StatusEnum.WaitingToConfirm)

                return (false, $"Order cannot be confirmed because its status is '{order.Status}'.");

            await _repository.ConfirmOrder(order);

            _notificationRepo.CreateNotificationTo(order.CustomerID,

               $"Order number {order.OrderNumber} has been confirmed.");

            return (true, $"Order number {order.OrderNumber} is confirmed and being prepared by restaurant.");

        }

        // Assign order to delivery man

        public async Task<bool> assignDelivaryManToOrder(Order order)

        {

            var result = await assignDelivaryManToOrderDetailed(order);

            return result.Success;

        }

        // assignDelivaryManToOrderDetailed must be public to implement interface

        public async Task<(bool Success, string Message)> assignDelivaryManToOrderDetailed(Order order)
        {
            if (order == null)
                return (false, "Order cannot be null.");

            if (order.Restaurant == null)
                return (false, "Restaurant details are not available for this order.");

            var deliveryMan = await _deliveryManService.GetClosestDeliveryManAsync(order.Restaurant.Latitude, order.Restaurant.Longitude);
            if (deliveryMan == null)
                return (false, "No delivery man available nearby. Please wait.");

            if (order.Status != StatusEnum.WaitingToConfirm)
                return (false, $"Order status must be '{StatusEnum.WaitingToConfirm}' to assign a delivery man.");

            await _repository.AssignOrderToDelivaryMan(order, deliveryMan.DeliveryManID);
            _notificationRepo.CreateNotificationTo(deliveryMan.DeliveryManID,
                $"Order number {order.OrderNumber} has been assigned to you for delivery.");
            deliveryMan.AvailabilityStatus = false;
            return (true, $"Delivery man {deliveryMan.DeliveryManID} assigned successfully.");
        }


        // Dashboard Summary

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)

        {

            return await _repository.GetDashboardSummaryAsync(restaurantId);

        }

        // Customer - Checkout

        public async Task<CheckoutViewDTO> Checkout(ShoppingCart shoppingCart)

        {

            if (shoppingCart.ShoppingCartItems == null || shoppingCart.ShoppingCartItems.Count <= 0)

                throw new ArgumentException("ShoppingCart is empty");

            if (shoppingCart.Restaurant == null)

                throw new ArgumentException("ShoppingCart isn't assigned to Restaurant");

            if (shoppingCart.Customer == null)

                throw new ArgumentException("ShoppingCart isn't assigned to Customer");

            Address add = await _addressRepo.getDafaultAddress(shoppingCart.CustomerID);

            if (add == null)

                throw new ArgumentException("There are no addresses for this user, please add an address and try again");

            if (string.IsNullOrEmpty(shoppingCart.Customer.User.PhoneNumber))

                throw new ArgumentException("There is no phone number for this user, please add phone and try again");

            try

            {

                TimeSpan delivaryDuration = await _openRouteService.GetTravelDurationAsync(

                   shoppingCart.Restaurant.Latitude,

                   shoppingCart.Restaurant.Longitude,

                   add.Latitude,

                   add.Longitude

                );

                var priceids = shoppingCart.ShoppingCartItems.Select(SC => SC.Item.StripePriceId).ToList();

                shoppingCart.ShoppingCartItems.Select(sc => sc.Quantity);

                CheckoutViewDTO checkout = _mapper.Map<CheckoutViewDTO>(shoppingCart);

                var addressdto = await _addressRepo.getDafaultAddress(shoppingCart.CustomerID);

                checkout.Address = _mapper.Map<AddressViewDto>(addressdto);

                checkout.PaymentLink = stripeService.CreatePaymentLink(shoppingCart.ShoppingCartItems.ToList());

                return checkout;

            }

            catch (Exception ex)

            {

                throw new ArgumentException(ex.Message);

            }

        }

        public async Task transferItemsFromCartToOrder(ShoppingCart cart, Order order)

        {

            foreach (var item in cart.ShoppingCartItems)

            {

                OrderItem orderitem = _mapper.Map<OrderItem>(item);

                orderitem.OrderID = order.OrderID;

                await _repository.AddOrderItem(orderitem);

            }

        }

        public async Task PlaceOrder(ShoppingCart cart)

        {

            if (cart.ShoppingCartItems == null || cart.ShoppingCartItems.Count <= 0)

                throw new ArgumentException("ShoppingCart is empty");

            if (cart.Restaurant == null)

                throw new ArgumentException("ShoppingCart isn't assigned to Restaurant");

            if (cart.Customer == null)

                throw new ArgumentException("ShoppingCart isn't assigned to Customer");

            Address add = await _addressRepo.getDafaultAddress(cart.CustomerID);

            if (add == null)

                throw new ArgumentException("There are no addresses for this user, please add an address and try again");

            if (string.IsNullOrEmpty(cart.Customer.User.PhoneNumber))

                throw new ArgumentException("There is no phone number for this user, please add phone and try again");

            using var transaction = await _repository.BeginTransactionAsync();

            try

            {

                Order order = _mapper.Map<Order>(cart);

                order.AddressID = add.AddressID;

                order.PhoneNumber = cart.Customer.User.PhoneNumber;

                TimeSpan delivaryDuration = await _openRouteService.GetTravelDurationAsync(

                    cart.Restaurant.Latitude,

                    cart.Restaurant.Longitude,

                    add.Latitude,

                    add.Longitude

                );

                order.OrderTimeToComplete = cart.Restaurant.orderTime + delivaryDuration;

                order.DelivaryPrice = cart.Restaurant.DelivaryPrice;

                order.OrderDate = DateTime.UtcNow;

                await _repository.AddOrder(order);

                foreach (var item in cart.ShoppingCartItems)

                {

                    OrderItem orderItem = _mapper.Map<OrderItem>(item);

                    orderItem.OrderID = order.OrderID;

                    await _repository.AddOrderItem(orderItem);

                }

                await _shoppingCartService.Clear(cart.CartID);

                // If promo codes feature is active, uncomment and adapt below:

                /*

                if (orderdto.PromoCodeID != null)

                {

                    bool applied = await _promoCodeService.ApplyPromoCode(order);

                    if (!applied)

                        throw new ArgumentException("Problem while applying PromoCode");

                }

                */

                // Payment processing could be added here

                await _repository.saveChangesAsync();

                await transaction.CommitAsync();

            }

            catch (Exception ex)

            {

                await transaction.RollbackAsync();

                throw new ArgumentException(ex.Message);

            }

        }

        public async Task<List<OrderViewDTO>> getOrders(string customerId)

        {

            List<Order> orders = await _repository.getOrders(customerId);

            List<OrderViewDTO> orderResult = _mapper.Map<List<OrderViewDTO>>(orders);

            return orderResult;

        }

        public async Task<List<OrderViewDTO>> GetOrdersByStatusAsyncForCustomer(string customerId, StatusEnum[] statuses)
        {
            var orders = await _repository.getOrders(customerId);

            // Filter by status using case-insensitive comparison
            var filteredOrders = orders.Where(o => statuses.Contains(o.Status));

            return _mapper.Map<List<OrderViewDTO>>(filteredOrders);
        }
        public async Task<OrderDetailDTO?> getOrderDetails(Guid orderId)
        {
            Order order = await _repository.GetOrderDetails(orderId);
            return _mapper.Map<OrderDetailDTO>(order);
        }

        public async Task<Order?> getOrder(Guid orderId)

        {

            return await _repository.GetOrderDetails(orderId); // entity returned directly

        }

        public async Task<List<DelivaryOrderDTO>> getPreparingOrdersForDelivarMan(string DelivaryId)

        {

            List<Order> orders = await _repository.getOrdersDelivaryMan(DelivaryId,StatusEnum.Preparing);

            List<DelivaryOrderDTO> orderResult = _mapper.Map<List<DelivaryOrderDTO>>(orders);

            return orderResult;

        }
        public async Task<List<DeliveryManUpdateOrderStatusDTO>> getDelivaredOrdersForDelivarMan(string DelivaryId)

        {

            List<Order> orders = await _repository.getOrdersDelivaryMan(DelivaryId,StatusEnum.Delivered);

            List<DeliveryManUpdateOrderStatusDTO> orderResult = _mapper.Map<List<DeliveryManUpdateOrderStatusDTO>>(orders);

            return orderResult;

        }
    }

}
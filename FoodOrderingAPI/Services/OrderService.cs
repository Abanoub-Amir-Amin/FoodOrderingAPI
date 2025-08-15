using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Services
{
    public class OrderService:IOrderService
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


        public OrderService
            (IOrderRepo repository,
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
            IStripeService stripeService)
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
        public async Task<IEnumerable<Order>> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            return await _repository.GetAllOrdersByRestaurantAsync(restaurantId);
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string restaurantId, StatusEnum[] statuses)
        {
            var orders = await _repository.GetAllOrdersByRestaurantAsync(restaurantId);

            // Filter by status using case-insensitive comparison
            var filteredOrders = orders.Where(o => statuses.Contains(o.Status));

            return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders);
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, StatusEnum newStatus, string restaurantId)
        {
            var allowedStatuses = new[] {
                StatusEnum.Preparing,
                StatusEnum.Out_for_Delivery,
                StatusEnum.Cancelled,
                StatusEnum.WaitingToConfirm
            };

            if (!allowedStatuses.Contains(newStatus))
                throw new ArgumentException("Invalid order status.");

            var order = await _repository.getOrderDetails(orderId);
            if (order == null)
                throw new ArgumentException("Order not found.");

            if (order.RestaurantID != restaurantId)
                throw new UnauthorizedAccessException("This restaurant does not own the order.");

            var oldStatus = order.Status;

            if (oldStatus == StatusEnum.WaitingToConfirm && newStatus == StatusEnum.Preparing)
            {
                bool confirmed = await ConfirmOrder(order);
                if (!confirmed)
                    throw new InvalidOperationException("Order confirmation failed.");

                order.Status = StatusEnum.Preparing;
                await _repository.saveChangesAsync();
            }
            else if (oldStatus == StatusEnum.WaitingToConfirm && newStatus == StatusEnum.Cancelled)
            {
                bool cancelled = await CancelOrder(order, "Cancelled by restaurant");
                if (!cancelled)
                    throw new InvalidOperationException("Order cancellation failed.");
            }
            else
            {
                order.Status = newStatus;
                await _repository.saveChangesAsync();
            }

            if (newStatus == StatusEnum.Out_for_Delivery)
            {
                _notificationRepo.CreateNotificationTo(order.CustomerID,
                    $"Order number {order.OrderNumber} is out for Out for Delivery");
            }

            return order;
        }

        //cancel order from restaurant
        public async Task<bool> CancelOrder(Order order,string reason)
        {

            if (order == null || order.Status != StatusEnum.WaitingToConfirm)
                return false;

            await _repository.CancelOrder(order);

            //this code to return payment to customer
            // if order payment method visa
            //var options = new RefundCreateOptions
            //{
            //    PaymentIntent = order.PaymentIntentId,
            //};
            //var service = new RefundService();
            //Refund refund = await service.CreateAsync(options);

            _notificationRepo.CreateNotificationTo(order.CustomerID,
                $"Order number {order.OrderNumber} cancelled,\n Reason: {reason}");

            return true;
        }
        public async Task<bool> ConfirmOrder(Order order)
        {
            if (order == null || order.Status != StatusEnum.WaitingToConfirm)
                return false;

            await _repository.ConfirmOrder(order);

            _notificationRepo.CreateNotificationTo(order.CustomerID,
               $"Order number {order.OrderNumber} Confirmed");
            return true;
        }



        //assign order to delivaryMan
        //in controller in confirm action we should assign delivaryman first and ensures that there are available delivaryman
        public async Task<bool> assignDelivaryManToOrder(Order order)
        {
            var deliveryMan = await _deliveryManService.GetClosestDeliveryManAsync(order.Restaurant.Latitude, order.Restaurant.Longitude);
            if (deliveryMan == null) return false;
            if (order.Status == StatusEnum.WaitingToConfirm) { 
                await _repository.AssignOrderToDelivaryMan(order, deliveryMan.DeliveryManID);

                _notificationRepo.CreateNotificationTo(order.DeliveryManID,
               $"Order number {order.OrderNumber} has assigned to deliver ");
                deliveryMan.AvailabilityStatus = false;
        }
            return true;

        }


        //Dashboard Summary
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string restaurantId)
        {
            return await _repository.GetDashboardSummaryAsync(restaurantId);
        }



        //customer
        public async Task<CheckoutViewDTO> Checkout (ShoppingCart shoppingCart)
        {
            var priceids = shoppingCart.ShoppingCartItems.Select(SC => SC.Item.StripePriceId).ToList();
            shoppingCart.ShoppingCartItems.Select(sc => sc.Quantity);
            CheckoutViewDTO checkout = _mapper.Map<CheckoutViewDTO>(shoppingCart);
            checkout.Address = await _addressRepo.getDafaultAddress(shoppingCart.CustomerID);
            checkout.PaymentLink = stripeService.CreatePaymentLink(shoppingCart.ShoppingCartItems.ToList());
            return checkout;
        }
        public async Task transferItemsFromCartToOrder(ShoppingCart cart, Order order)
        {
            foreach(var item in cart.ShoppingCartItems)
            {
                OrderItem orderitem = _mapper.Map<OrderItem>(item);
                orderitem.OrderID=order.OrderID;
                await _repository.AddOrderItem(orderitem);
            }
        }
        public async Task PlaceOrder(ShoppingCart cart)
        {
            // التأكد من صحة البيانات المدخلة
            if (cart.Restaurant == null)
                throw new ArgumentException("ShoppingCart isn't assigned to Restaurant");

            if (cart.ShoppingCartItems == null || cart.ShoppingCartItems.Count <= 0)
                throw new ArgumentException("ShoppingCart is empty");

            if (cart.Customer == null)
                throw new ArgumentException("ShoppingCart isn't assigned to Customer");
            Address add = await _addressRepo.getDafaultAddress(cart.CustomerID);
            if (add == null)
                throw new ArgumentException("there no Addresses for this user, pleases add address and try again");

            if(string.IsNullOrEmpty(cart.Customer.User.PhoneNumber))
                throw new ArgumentException("there no phone number for this user, pleases add phone and try again");

            //transacation 
            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                //create order using shoppingcart and data from frontend
                Order order = _mapper.Map<Order>(cart);
                //_mapper.Map(orderdto, order); // apply dto overrides
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

                //if (orderdto.PromoCodeID != null)
                //{
                //    bool applied = await _promoCodeService.ApplyPromoCode(order);
                //    if (!applied)
                //        throw new ArgumentException("Problem while applying PromoCode");
                //}

                // you can add payment here
                // await _paymentService.ChargeAsync(order);

                await _repository.saveChangesAsync();
                await transaction.CommitAsync();
            }
            catch(Exception ex) 
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
            Order order = await _repository.getOrderDetails(orderId);
            return _mapper.Map<OrderDetailDTO>(order);
        }
        public async Task<Order?> getOrder(Guid orderId)
        {
            Order order = await _repository.getOrderDetails(orderId);
            return order;
        }
        public async Task<List<DelivaryOrderDTO>> getOrdersForDelivarMan(string DelivaryId)
        {
            List<Order> orders = await _repository.getOrdersDelivaryMan(DelivaryId);
            List<DelivaryOrderDTO> orderResult = _mapper.Map<List<DelivaryOrderDTO>>(orders);
            return orderResult;
        }
    }
}


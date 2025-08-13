using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class DelivaryOrderDTO
    {
        public Guid OrderID { get; set; }
        public int OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string? RestaurantPhone { get; set; }

        public List<OrderItemDto> items { get; set; }
        public DateTime OrderDate { get; set; }
        //public String PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }

    }
}

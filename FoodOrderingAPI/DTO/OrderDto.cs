using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodOrderingAPI.DTO
{
    public class OrderDto
    {
        [BindNever]
        public Guid OrderID { get; set; }
        public Guid AddressID { get; set; }
        public string RestaurantID { get; set; } 
        public string? DeliveryManID { get; set; } 
        public StatusEnum Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid? PromoCodeID { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}

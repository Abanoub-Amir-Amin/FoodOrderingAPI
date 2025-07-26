namespace FoodOrderingAPI.DTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public string CustomerID { get; set; }  
        public int AddressID { get; set; }
        public string RestaurantID { get; set; } 
        public string? DeliveryManID { get; set; } 
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public decimal TotalPrice { get; set; }
        public int? PromoCodeID { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}

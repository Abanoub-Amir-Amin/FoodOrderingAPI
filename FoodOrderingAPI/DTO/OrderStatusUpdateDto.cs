using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class OrderStatusUpdateDto
    {
        public Guid OrderID { get; set; }
        public StatusEnum Status { get; set; }
    }
}

using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class OrderStatusUpdateDto
    {
        public int OrderID { get; set; }
        public StatusEnum Status { get; set; }
    }
}

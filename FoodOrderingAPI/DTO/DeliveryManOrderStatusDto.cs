using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManOrderStatusDto
    {
        public Guid OrderID { get; set; }
        public StatusEnum Status { get; set; }
        public string DeliveryManId { get; set; }
    }
}

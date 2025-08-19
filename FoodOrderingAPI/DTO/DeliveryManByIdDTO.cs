using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManByIdDTO
    {
        public string DeliveryManID { get; set; }
        public AccountStatusEnum AccountStatus { get; set; }
    }
}

using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManDto
    {
        public string DeliveryManId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public float? Rank { get; set; }
        public bool AvailabilityStatus { get; set; }

        public UserDto User { get; set; }
    }
}
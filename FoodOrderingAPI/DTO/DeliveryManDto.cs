using FoodOrderingAPI.Models;
using System.Text.Json.Serialization;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManDto
    {
        public string DeliveryManId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public float? Rank { get; set; }
        public bool AvailabilityStatus { get; set; }

        [JsonIgnore]
        public AccountStatusEnum AccountStatus { get; set; } = AccountStatusEnum.Pending;
        public UserDto User { get; set; }
    }
}
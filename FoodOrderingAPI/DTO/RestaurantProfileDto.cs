using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantProfileDto
    {
        [MaxLength(100)]
        public string RestaurantName { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        [MaxLength(100)]
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }

        [MaxLength(500)]
        public string LogoUrl { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

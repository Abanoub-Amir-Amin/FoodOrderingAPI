using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantUpdateDto
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }
        public IFormFile LogoFile { get; set; }  // file upload for update
        public string Email { get; set; }
        public string Phone { get; set; }
        //update to restaurant to get time of order to deliver to customer
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DelivaryPrice { get; set; }
        public TimeSpan orderTime { get; set; }
        public UserDto User { get; set; }
    }
}

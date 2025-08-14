using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantUpdateDto
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }
        public IFormFile? LogoFile { get; set; }  // file upload for update
        //update to restaurant to get time of order to deliver to customer

        [BindNever]
        public string? ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DelivaryPrice { get; set; }
        public TimeSpan orderTime { get; set; }
        public UserDto User { get; set; }

    }
}

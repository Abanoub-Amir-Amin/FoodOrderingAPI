using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantDto
    {
        [BindNever]
        public string? RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public string ImageFile { get; set; }
        public UserDto User { get; set; }
        //update to restaurant to get time of order to deliver to customer
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan orderTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;

    }
}

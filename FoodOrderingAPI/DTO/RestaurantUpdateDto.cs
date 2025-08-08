using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantUpdateDto
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }
        //update to restaurant to get time of order to deliver to customer
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan orderTime { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;

        public IFormFile? LogoFile { get; set; }  
        public UserDto User { get; set; }
    }
}

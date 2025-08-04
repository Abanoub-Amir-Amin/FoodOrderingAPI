using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantUpdateDto
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }
        public IFormFile? LogoFile { get; set; }  
        public UserDto User { get; set; }
    }
}

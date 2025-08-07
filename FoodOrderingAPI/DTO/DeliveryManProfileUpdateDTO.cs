using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManProfileUpdateDTO
    {
        public string UserName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Must start with 01 and be 11 digits.")]
        public string PhoneNumber { get; set; }

    }
}

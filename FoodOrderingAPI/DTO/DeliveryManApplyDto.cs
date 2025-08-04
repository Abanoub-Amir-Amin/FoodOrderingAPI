using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class DeliveryManApplyDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Must start with 01 and be 11 digits.")]
        public string? PhoneNumber { get; set; }

        public bool AgreeTerms { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
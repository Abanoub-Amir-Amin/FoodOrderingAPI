using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class UpdateCustomerDTO
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string? PhoneNumber { set; get; }
        public GenderEnum? Gender { set; get; }
    }
}

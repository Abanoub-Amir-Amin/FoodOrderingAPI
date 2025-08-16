using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.DTO
{
    public class UserDto
    {
        public UserDto() { }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Password { get; set; }
    }
}

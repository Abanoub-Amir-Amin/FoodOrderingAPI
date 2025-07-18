using FoodOrderingAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [Compare("Email")]
        public string EmailConfirmation { get; set; }
        public string Phone { get; set; }
        [EnumDataType(typeof(RoleEnum))]
        public RoleEnum Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        [EnumDataType(typeof(RoleEnum))]
        public RoleEnum Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "User name cannot be empty.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password cannot be empty.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email cannot be empty.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "User phone cannot be empty.")]
        public string Phone { get; set; }
        [Required]
        [EnumDataType(typeof(RoleEnum))]
        public RoleEnum Role { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public Guid? GroupId { get; set; }
        public IEnumerable<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual Group? Group { get; set; }
    }
}

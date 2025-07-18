using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        [Required(ErrorMessage = "Notification message cannot be empty.")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Notification type cannot be empty.")]
        public string Type { get; set; }
        public bool? IsRead { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public string? UserId { get; set; }
        public virtual User User { get; set; }
    }
}

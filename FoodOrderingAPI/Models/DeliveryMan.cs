using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class DeliveryMan
    {
        [Key, ForeignKey(nameof(User))]
        public int DeliveryManID { get; set; }

        [MaxLength(255)]
        public string CurrentLocation { get; set; }

        public int Rank { get; set; } = 0;

        public bool AvailabilityStatus { get; set; } = true;

        public User User { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<DeliveryReport> DeliveryReports { get; set; }
    }
}

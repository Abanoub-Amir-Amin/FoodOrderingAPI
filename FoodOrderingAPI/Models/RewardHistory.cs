using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class RewardHistory
    {
        [Key]
        public int RewardID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }

        public int PointsEarned { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; }

        public DateTime DateEarned { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }
    }
}

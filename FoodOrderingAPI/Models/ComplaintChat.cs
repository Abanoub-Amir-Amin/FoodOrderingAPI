using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ComplaintChat
    {
        [Key]
        public int ChatID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }

        [ForeignKey(nameof(Admin))]
        public int AdminID { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }
        public Admin Admin { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}

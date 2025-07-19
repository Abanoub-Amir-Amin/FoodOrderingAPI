using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderID { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemID { get; set; }

        public int Quantity { get; set; }

        [MaxLength(255)]
        public string Preferences { get; set; }

        public Order Order { get; set; }

        public Item Item { get; set; }
    }
}

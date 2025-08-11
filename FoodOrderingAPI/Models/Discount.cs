using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public string RestaurantID { get; set; }

        [ForeignKey(nameof(Item))]
        public Guid ItemID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Percentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Restaurant Restaurant { get; set; }

        public Item Item { get; set; }
    }
}

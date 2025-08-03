using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public Guid CartItemID { get; set; }

        [ForeignKey(nameof(ShoppingCart))]
        public Guid CartID { get; set; }

        [ForeignKey(nameof(Item))]
        public Guid ItemID { get; set; }

        public int Quantity { get; set; } = 1;

        [MaxLength(255)]
        public string? Preferences { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        public Item Item { get; set; }
    }
}

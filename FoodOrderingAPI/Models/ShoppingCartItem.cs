using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int CartItemID { get; set; }

        [ForeignKey(nameof(ShoppingCart))]
        public int CartID { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemID { get; set; }

        public int Quantity { get; set; } = 1;

        [MaxLength(255)]
        public string Preferences { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAtAddition { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        public Item Item { get; set; }
    }
}

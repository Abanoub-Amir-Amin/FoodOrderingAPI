using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Item
    {
        [Key]
        public int ItemID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public int RestaurantID { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(50)]
        public string Category { get; set; }

        public Restaurant Restaurant { get; set; }

        public ICollection<Discount> Discounts { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}

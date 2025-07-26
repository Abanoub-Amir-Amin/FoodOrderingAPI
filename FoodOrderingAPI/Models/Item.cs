using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Item
    {
        [Key]
        public Guid ItemID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public string RestaurantID { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        // For icon or thumbnail if needed
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(50)]
        public string Category { get; set; }

        public Restaurant Restaurant { get; set; }

        public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}

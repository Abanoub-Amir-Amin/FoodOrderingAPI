using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ShoppingCart
    {
        [Key]
        public Guid CartID { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0;

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public Customer Customer { get; set; }

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}

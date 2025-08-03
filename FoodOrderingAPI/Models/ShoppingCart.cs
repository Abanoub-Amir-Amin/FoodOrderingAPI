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
        
        //to ensurre that items must be from one Restaurant
        [ForeignKey(nameof(Restaurant))]
        public string? RestaurantID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;


        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAfterDiscount => SubTotal + DelivaryPrice - DiscountAmount; 

        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Review
    {
        [Key]
        public Guid ReviewID { get; set; }

        [ForeignKey(nameof(Order))]
        public Guid OrderID { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public string RestaurantID { get; set; }
        public string UserName { get; set; }
        public float Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}

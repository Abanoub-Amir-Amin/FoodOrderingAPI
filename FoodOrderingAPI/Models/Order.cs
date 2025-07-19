using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }

        [ForeignKey(nameof(Address))]
        public int AddressID { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public int RestaurantID { get; set; }

        [ForeignKey(nameof(DeliveryMan))]
        public int? DeliveryManID { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? DeliveredAt { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey(nameof(PromoCode))]
        public int? PromoCodeID { get; set; }

        public Customer Customer { get; set; }
        public Address Address { get; set; }
        public Restaurant Restaurant { get; set; }
        public DeliveryMan DeliveryMan { get; set; }
        public PromoCode PromoCode { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<DeliveryReport> DeliveryReports { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}

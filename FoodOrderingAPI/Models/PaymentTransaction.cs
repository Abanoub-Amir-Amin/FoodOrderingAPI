using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class PaymentTransaction
    {
        [Key]
        public Guid PaymentTransactionID { get; set; }

        [ForeignKey(nameof(Order))]
        public Guid OrderID { get; set; }

        [ForeignKey(nameof(PaymentMethod))]
        public Guid? PaymentMethodID { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }  // Pending, Completed, Failed, Refunded

        [MaxLength(100)]
        public string TransactionReference { get; set; }

        public Order Order { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class PaymentMethod
    {
        [Key]
        public Guid PaymentMethodID { get; set; }

        [ForeignKey(nameof(Customer))]
        public string CustomerID { get; set; }

        [Required, MaxLength(50)]
        public string MethodType { get; set; }  // e.g. CreditCard, PayPal, Cash

        [MaxLength(100)]
        public string Provider { get; set; }   // e.g. Visa, MasterCard

        [MaxLength(100)]
        public string AccountNumber { get; set; }  // masked/tokenized

        public DateTime? ExpiryDate { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }

        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}

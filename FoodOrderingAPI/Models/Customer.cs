using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace FoodOrderingAPI.Models
{
    public class Customer
    {
        
        [Key]
        public string CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderEnum ?Gender { get; set; }
        public int LoyaltyPoints { get; set; } = 0;

        //public int TotalOrders { get; set; } = 0;
        [ForeignKey(nameof(User))]
        public string UserID {  get; set; }
        public User User { get; set; }
        public ICollection<Address>Addresses { get; set; }
        public ICollection<RewardHistory> RewardHistories { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ComplaintChat> ComplaintChats { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}

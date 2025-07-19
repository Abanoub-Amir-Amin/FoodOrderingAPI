using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace FoodOrderingAPI.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        public int LoyaltyPoints { get; set; } = 0;

        public int TotalOrders { get; set; } = 0;
        [ForeignKey(nameof(User))]
        public int UserID {  get; set; }
        public User User { get; set; }
        public ICollection<RewardHistory> RewardHistories { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ComplaintChat> ComplaintChats { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}

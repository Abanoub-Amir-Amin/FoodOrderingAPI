using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class PromoCode
    {
        [Key]
        public Guid PromoCodeID { get; set; }
        public string RestaurantID { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        public float DiscountPercentage { get; set; }

        public bool IsFreeDelivery { get; set; }

        [MaxLength(20)]
        public string IssuedByType { get; set; }  

        public string IssuedByID { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int UsageLimit { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}

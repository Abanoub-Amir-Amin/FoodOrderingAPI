using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Restaurant
    {
        [Key, ForeignKey(nameof(User))]
        public string RestaurantID { get; set; }
        public string UserId { get; set; }

        [MaxLength(100)]
        public string RestaurantName { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }
        //update to restaurant to get time of order to deliver to customer
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan orderTime { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;
        public float? Rating { get; set; }

        [MaxLength(100)]
        public string OpenHours { get; set; }

        public bool IsActive { get; set; } = false;

        // Indicates if the restaurant is currently accepting orders or bussy
        public bool IsAvailable { get; set; } = true; // Default to true

        // URL for the restaurant's logo
        [MaxLength(500)] 
        public string LogoUrl { get; set; }

        public User User { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
        public ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

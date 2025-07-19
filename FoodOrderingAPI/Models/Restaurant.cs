using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class Restaurant
    {
        [Key, ForeignKey(nameof(User))]
        public int RestaurantID { get; set; }

        [MaxLength(100)]
        public string RestaurantName { get; set; }

        [MaxLength(255)]
        public string Location { get; set; }

        public float? Rating { get; set; }

        [MaxLength(100)]
        public string OpenHours { get; set; }

        public bool IsActive { get; set; } = true;

        public User User { get; set; }

        public ICollection<Item> Items { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }

    }
}

using FoodOrderingAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class ShoppingCartDTO
    {
        public Guid CartID { get; set; }


        //to ensurre that items must be from one Restaurant
        public string? RestaurantID { get; set; }
        public string? RestaurantName { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; } = 0;


        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        
        public decimal TotalAfterDiscount => SubTotal + DelivaryPrice ;

        public List<ShoppingCartItemDto> ShoppingCartItems { get; set; } = new List<ShoppingCartItemDto>();
    }
}

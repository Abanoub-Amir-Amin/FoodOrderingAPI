using FoodOrderingAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class ShoppingCartItemAddedDTO
    {
  
        public Guid CartID { get; set; }

        public Guid ItemID { get; set; }

        [MaxLength(255)]
        public string? Preferences { get; set; }

    }
}

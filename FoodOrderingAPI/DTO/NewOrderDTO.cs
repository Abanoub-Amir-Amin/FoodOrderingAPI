using FoodOrderingAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    /// <summary>
    /// DTO used to add a new order from the frontend.
    /// Contains customer ID, address ID, restaurant ID, optional promo code,
    /// subtotal, delivery price, discount amount, and list of order items.
    /// </summary>
    public class NewOrderDTO
    {
        //public string CustomerID { get; set; }
        [Required]
        public Guid AddressID { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        //public string RestaurantID { get; set; }
       

        public Guid? PromoCodeID { get; set; }

//        /*[Column(TypeName = "decimal(10,2)")]
//        public decimal SubTotal { get; set; } = 0;

//        [Column(TypeName = "decimal(10,2)")]
//        public decimal DelivaryPrice { get; set; } = 0;

//        [Column(TypeName = "decimal(10,2)")]
//        public decimal DiscountAmount { get; set; } = 0;
//*/
        //public ICollection<OrderItemAddedDto> OrderItems { get; set; } = new List<OrderItemAddedDto>();
        //public PaymentTransaction PaymentTransactions { get; set; }
        //public string PaymentIntentId { get; set; }//is this should save in order for payment

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class PromoCodeApplyDto
    {
        public Guid? PromoCodeID { get; set; }
        public bool IsVaild { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DelivaryPrice { get; set; } = 0;

        //update total price after delete discount amount
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice => SubTotal + DelivaryPrice - DiscountAmount;

        public string message { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace FoodOrderingAPI.DTO
{
    public class PromoCodeDto
    {
        [BindNever]
        public Guid? PromoCodeID { get; set; }
        public string? Code { get; set; }
        public float? DiscountPercentage { get; set; }
        public bool? IsFreeDelivery { get; set; }
        public string? IssuedByType { get; set; }
        public string? IssuedByID { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}

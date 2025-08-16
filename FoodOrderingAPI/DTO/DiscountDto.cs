using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace FoodOrderingAPI.DTO
{
    public class DiscountDto
    {
        [BindNever]
        public int? DiscountID { get; set; }
        public string? ItemName { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

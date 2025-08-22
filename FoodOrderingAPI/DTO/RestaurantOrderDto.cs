using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FoodOrderingAPI.DTO
{
    public class RestaurantOrderDto
    {
        //general Details
        [BindNever]
        public Guid OrderID { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public StatusEnum Status { get; set; }


        //order Details
        public List<OrderItemDto> items { get; set; }

        //Customer Details
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    } 
}

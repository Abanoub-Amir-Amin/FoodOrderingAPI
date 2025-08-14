using FoodOrderingAPI.Models;
using System.Diagnostics.Contracts;

namespace FoodOrderingAPI.DTO
{
    /// <summary>
    /// DTO used to present general order details to the customer, 
    /// including status, restaurant name, item names, order date, 
    /// payment method, and total price.
    /// </summary>
    public class OrderViewDTO
    {
        public Guid OrderID { get; set; }
        public int OrderNumber { get; set; }
        public StatusEnum Status { get; set; }
        public string RestaurantName { get; set; }
        public List<string> itemNames { get; set; }
        public DateTime OrderDate { get; set; }
        //public String PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }

    }
}

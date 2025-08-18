namespace FoodOrderingAPI.DTO
{
    public class DeliveryManUpdateOrderStatusDTO
    {
        public int OrderNumber { get; set; }

        public string Address { get; set; }

        public decimal TotalPrice { get; set; }

        public string UserName { set; get; }

        public DateTime? DeliveredAt { get; set; }
    }
}

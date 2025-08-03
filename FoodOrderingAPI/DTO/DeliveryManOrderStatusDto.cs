namespace FoodOrderingAPI.DTO
{
    public class DeliveryManOrderStatusDto
    {
        public Guid OrderID { get; set; }
        public string Status { get; set; }
        public string DeliveryManId { get; set; }
    }
}

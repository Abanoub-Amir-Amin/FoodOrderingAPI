namespace FoodOrderingAPI.DTO
{
    public class OrderItemDto
    {
        public Guid OrderID { get; set; }
        public Guid? orderItemId { get; set; }
        public string itemName {  get; set; }
        public int Quantity { get; set; }
        public string Preferences { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice { get; set; }

    }
}

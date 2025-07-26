namespace FoodOrderingAPI.DTO
{
    public class OrderItemDto
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public string Preferences { get; set; }
    }
}

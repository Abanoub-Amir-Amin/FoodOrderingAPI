namespace FoodOrderingAPI.DTO
{
    public class OrderItemDto
    {
        

        public Guid? OrderItemID { get; set; }
        public Guid OrderID { get; set; }
        public string itemName { get; set; }
        public int Quantity { get; set; }
        public string? ImageFile { get; set; }
        public string Preferences { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

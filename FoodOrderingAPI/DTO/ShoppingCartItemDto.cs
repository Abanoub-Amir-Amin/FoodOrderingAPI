namespace FoodOrderingAPI.DTO
{
    public class ShoppingCartItemDto
    {
        public Guid ShoppingCartItemId {  get; set; }
        public string? ImageFile { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Preferences { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

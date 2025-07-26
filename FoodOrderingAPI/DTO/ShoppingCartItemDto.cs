namespace FoodOrderingAPI.DTO
{
    public class ShoppingCartItemDto
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ItemID { get; set; }
        public int Quantity { get; set; }
        public string Preferences { get; set; }
        public decimal PriceAtAddition { get; set; }
    }
}

namespace FoodOrderingAPI.DTO
{
    public class ShoppingCartDto
    {
        public int CartID { get; set; }
        public string CustomerID { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public List<ShoppingCartItemDto> ShoppingCartItems { get; set; }
    }
}

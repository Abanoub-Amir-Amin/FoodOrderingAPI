namespace FoodOrderingAPI.DTO
{
    public class ReviewDto
    {
        public Guid OrderID { get; set; }
        public string CustomerID { get; set; } 
        public float Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

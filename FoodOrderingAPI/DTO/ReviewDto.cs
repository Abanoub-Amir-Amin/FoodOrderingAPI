namespace FoodOrderingAPI.DTO
{
    public class ReviewDto
    {
        public int ReviewID { get; set; }
        public int OrderID { get; set; }
        public string CustomerID { get; set; } 
        public string RestaurantID { get; set; }  
        public float Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

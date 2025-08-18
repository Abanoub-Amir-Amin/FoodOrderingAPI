namespace FoodOrderingAPI.DTO
{
    public class ReviewGetterDTO
    {
        public Guid ReviewID { get; set; }
        public Guid OrderID { get; set; }
        public string CustomerID { get; set; }
        public string UserName { get; set; }
        public string RestaurantID { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

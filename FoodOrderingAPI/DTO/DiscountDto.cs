namespace FoodOrderingAPI.DTO
{
    public class DiscountDto
    {
        public Guid itemID { get; set; }
        public string RestaurantName { get; set; }  
        public string ItemName { get; set; }
        public float Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

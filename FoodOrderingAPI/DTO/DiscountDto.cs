namespace FoodOrderingAPI.DTO
{
    public class DiscountDto
    {
        public int? DiscountID { get; set; }
        public string RestaurantID { get; set; }  
        public Guid ItemID { get; set; }
        public float Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

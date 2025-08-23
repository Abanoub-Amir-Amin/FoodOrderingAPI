namespace FoodOrderingAPI.DTO
{
    public class AllRestaurantsDTO
    {
        public string Id { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public decimal DelivaryPrice { get; set; } = 0;
        public float? Rating { get; set; }
        public string ImageFile { get; set; }
    }
}

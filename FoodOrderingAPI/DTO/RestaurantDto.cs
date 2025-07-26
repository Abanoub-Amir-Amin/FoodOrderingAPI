namespace FoodOrderingAPI.DTO
{
    public class RestaurantDto
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public float? Rating { get; set; }
        public string OpenHours { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsAvailable { get; set; } = true; 
        public string LogoUrl { get; set; } 
        public UserDto User { get; set; }
    }
}

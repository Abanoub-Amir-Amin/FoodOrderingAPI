namespace FoodOrderingAPI.DTO
{
    public class RestaurantUpdateDto
    {
        public string RestaurantName { get; set; }
        public string Location { get; set; }
        public string OpenHours { get; set; }
        public bool? IsAvailable { get; set; }
        public IFormFile LogoFile { get; set; }  // file upload for update
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

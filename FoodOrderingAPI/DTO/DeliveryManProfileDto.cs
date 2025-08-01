namespace FoodOrderingAPI.DTO
{
    public class DeliveryManProfileDto
    {
        public string UserName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public float? Rank { get; set; }
        public bool AvailabilityStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

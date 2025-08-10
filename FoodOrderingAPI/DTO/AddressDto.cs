using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.DTO
{
    public class AddressDTO
    {
        [MaxLength(50)]
        public string Label { get; set; }

        [MaxLength(255)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}

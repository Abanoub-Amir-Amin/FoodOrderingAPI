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
        [Range(-90,90)]
        public double Latitude { get; set; }
        [Range(-180,180)]
        public double Longitude { get; set; }

    }
}

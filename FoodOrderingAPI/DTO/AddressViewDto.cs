using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class AddressViewDto
    {
        [Key]
        public Guid AddressID { get; set; }
        public string? CustomerID { get; set; }

        [MaxLength(50)]
        public string Label { get; set; }

        [MaxLength(255)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]

        public bool IsDefault { get; set; } = false;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class DeliveryReport
    {
        [Key]
        public int ReportID { get; set; }

        [ForeignKey(nameof(DeliveryMan))]
        public int DeliveryManID { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderID { get; set; }

        [MaxLength(255)]
        public string StatusNote { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public DeliveryMan DeliveryMan { get; set; }

        public Order Order { get; set; }
    }

}

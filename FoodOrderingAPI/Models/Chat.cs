using System.ComponentModel.DataAnnotations;

namespace FoodOrderingAPI.Models
{
    public class Chat
    {
        public int id { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        public string ConversationId { get; set; }
    }
}

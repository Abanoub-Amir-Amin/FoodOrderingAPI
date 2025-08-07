using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid MessageID { get; set; }

        [ForeignKey(nameof(ComplaintChat))]
        public Guid ChatID { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public ComplaintChat ComplaintChat { get; set; }
    }
}

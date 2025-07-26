using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageID { get; set; }

        [ForeignKey(nameof(ComplaintChat))]
        public int ChatID { get; set; }

        //[ForeignKey(nameof(User))]
        //public int SenderID { get; set; }

        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public ComplaintChat ComplaintChat { get; set; }

        //public User Sender { get; set; }
    }
}

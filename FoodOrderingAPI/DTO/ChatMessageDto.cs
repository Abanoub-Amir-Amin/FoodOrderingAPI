namespace FoodOrderingAPI.DTO
{
    public class ChatMessageDto
    {
        public int MessageID { get; set; }
        public int ChatID { get; set; }
        public string SenderID { get; set; } 
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
    }
}

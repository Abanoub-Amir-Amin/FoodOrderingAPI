namespace FoodOrderingAPI.DTO
{
    public class ComplaintChatDto
    {
        public int ChatID { get; set; }
        public string CustomerID { get; set; }
        public string AdminID { get; set; }
        public DateTime StartedAt { get; set; }
        public List<ChatMessageDto> ChatMessages { get; set; }
    }
}

namespace FoodOrderingAPI.DTO
{
    public class ChatDTO
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string CustomerId { get; set; } // used to specify which complaintChat table will be used to add new messages.
        public string Message { get; set; }
    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ChatController : ControllerBase
    {
        public IHubContext<ChatHub> HubContext { get; }
        public ApplicationDBContext DBContext { get; }

        public ChatController(IHubContext<ChatHub> hubContext, ApplicationDBContext dBContext)
        {
            HubContext = hubContext;
            DBContext = dBContext;
        }
        [HttpPost("SendMessage")]
        public IActionResult SendMessage([FromBody] ChatDTO dto)
        {
            try
            {
                if (dto.Message == null || string.IsNullOrEmpty(dto.Message))
                {
                    return BadRequest(new { error = "Invalid message data." });
                }

                Console.WriteLine($"Sender: {dto.SenderId}, Receiver: {dto.ReceiverId}, Customer: {dto.CustomerId}, Message: {dto.Message}");

                var userChat = DBContext.ComplaintChats.FirstOrDefault(c => c.CustomerID == dto.CustomerId);

                if (userChat == null)
                {
                    // Return JSON error instead of plain text
                    return BadRequest(new { error = "Chat not found." });
                }

                // Send the message via SignalR
                HubContext.Clients.User(dto.ReceiverId).SendAsync("ReceiveMessage", dto.SenderId, dto.Message);

                var message = new ChatMessage
                {
                    ChatID = userChat.ChatID,
                    MessageText = dto.Message,
                    ReceiverID = dto.ReceiverId,
                    SenderID = dto.SenderId,
                    SentAt = DateTime.Now,
                };

                DBContext.ChatMessages.Add(message);
                DBContext.SaveChanges();

                return Ok(new
                {
                    success = true,
                    senderId = dto.SenderId,
                    receiverId = dto.ReceiverId,
                    message = dto.Message,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
        [HttpGet("GetChatMessages/{userId}")]
        public IActionResult GetChatMessages(string userId)
        {
            var userChat = DBContext.ComplaintChats.FirstOrDefault(c => c.CustomerID == userId);
            if (userChat == null)
            {
                return Ok("Chat not found.");
            }
            var messages = DBContext.ChatMessages
                .Where(m => m.ChatID == userChat.ChatID && (m.ReceiverID == userId || m.SenderID == userId))
                .OrderBy(m => m.SentAt)
                .ToList();
            List<MsgHistoryResponse> msgs = new List<MsgHistoryResponse>();
            foreach (var message in messages)
            {
                msgs.Add(new MsgHistoryResponse
                {
                    Sender = message.SenderID,
                    Message = message.MessageText,
                });
            }
            return Ok(msgs);
        }
        [HttpGet("id")]
        public IActionResult GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(userId);
        }
    }
}

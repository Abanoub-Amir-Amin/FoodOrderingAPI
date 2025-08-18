using FoodOrderingAPI.Models;
using FoodOrderingAPI.services;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {

        private readonly IChatService _chatService;
        private readonly IEmbeddingService _embeddingService;
        public ChatBotController(IChatService chatService , IEmbeddingService embeddingService)
        {
            _chatService = chatService;
            _embeddingService = embeddingService;
        }
        [HttpPut]
        public async Task<IActionResult> SaveMessage([FromBody] Chat chat)
        {
            if (string.IsNullOrWhiteSpace(chat.Message) || string.IsNullOrWhiteSpace(chat.Sender) || string.IsNullOrWhiteSpace(chat.ConversationId))
                return BadRequest("Sender, Message, and ConversationId required");

            var saved = await _chatService.SaveMessageAsync(chat.Sender, chat.Message, chat.ConversationId);
            return Ok(saved);
        }

        [HttpGet("sender")]
        public async Task<IActionResult> GetMessages([FromQuery] string sender, [FromQuery] string conversationId)
        {
            if (string.IsNullOrWhiteSpace(sender))
                return BadRequest("Sender is required");

            var messages = await _chatService.GetMessagesAsync(sender, conversationId);
            return Ok(messages);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string sender, [FromQuery] string conversationId)
        {
            if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(conversationId))
                return BadRequest("Sender and conversationId required");

            var messages = await _chatService.GetMessagesAsync(sender, conversationId);
            return Ok(messages);
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetConversationSessions([FromQuery] string sender)
        {
            if (string.IsNullOrWhiteSpace(sender))
                return BadRequest("Sender is required");

            var sessions = await _chatService.GetConversationSessionsAsync(sender);
            return Ok(sessions);
        }

    }
}

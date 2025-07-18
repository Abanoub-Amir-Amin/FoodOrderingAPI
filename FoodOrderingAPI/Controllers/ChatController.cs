using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        public IHubContext<ChatHub> HubContext { get; }
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            HubContext = hubContext;
        }
        [HttpPost("SendMessage")]
        public IActionResult SendMessage([FromBody] ChatDTO dto)
        {
            if (dto.Message == null || string.IsNullOrEmpty(dto.Message) || string.IsNullOrEmpty(dto.Message))
            {
                return BadRequest("Invalid message data.");
            }
            HubContext.Clients.User(dto.UserId).SendAsync("ReceiveMessage", dto.Message);
            return Ok(new { dto.UserId, dto.Message });
        }
    }
}

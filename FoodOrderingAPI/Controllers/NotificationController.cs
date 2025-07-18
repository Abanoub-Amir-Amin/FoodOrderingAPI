using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public INotificationRepo NotificationRepo { get; }
        public NotificationController(INotificationRepo notificationRepo)
        {
            NotificationRepo = notificationRepo;
        }

        [HttpPost("Notify")]
        public IActionResult SendNotificationTo([FromBody] NotificationDTO dto)
        {
            NotificationRepo.CreateNotificationTo(dto.UserId, dto.Message);
            return Ok(new {dto.UserId, dto.Message});
        }

        [HttpPost("NotifyAll")]
        public IActionResult SendNotificationToAll([FromBody] mDTO dTO)
        {
            NotificationRepo.CreateNotificationToAll(dTO.Message);
            return Ok(new { Message = dTO.Message });
        }

    }
    public class mDTO
    {
        public string Message { get; set; }
    }
}

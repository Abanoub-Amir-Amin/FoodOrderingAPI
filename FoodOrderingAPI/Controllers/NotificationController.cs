using FoodOrderingAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpGet]
        public IActionResult SendNotificationTo([FromBody] NotificationDTO dto)
        {
            return Ok("Test endpoint is working!");
        }
    }
}

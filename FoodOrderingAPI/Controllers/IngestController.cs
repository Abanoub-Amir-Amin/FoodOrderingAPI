using FoodOrderingAPI.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngestController : ControllerBase
    {
        private readonly KnowledgeIngestionService _ingestionService;

        public IngestController(KnowledgeIngestionService ingestionService)
        {
            _ingestionService = ingestionService;
        }

        //send and save embedding in SQLServer
        [HttpPost("store")]
        public async Task<IActionResult> StoreText([FromBody] string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Content cannot be empty.");
            }

            try
            {
                await _ingestionService.LoadAndStoreAsync(content);
                return Ok(content);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Failed to store content: {ex.Message}");
            }

        }
    }

}

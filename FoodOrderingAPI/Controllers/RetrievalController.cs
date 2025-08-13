using FoodOrderingAPI.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetrievalController : ControllerBase
    {
        private readonly RetrievalService _retrievalService;
        private readonly IEmbeddingService _embeddingService;

        public RetrievalController(RetrievalService retrievalService, IEmbeddingService embeddingService)
        {
            _retrievalService = retrievalService;
            _embeddingService = embeddingService;
        }

        [HttpGet("query")]
        public async Task<IActionResult> Query([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query text cannot be empty.");

            try
            {
                var results = await _retrievalService.GetTopMatchesAsync(q);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Retrieval failed: {ex.Message}");
            }
        }

        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Question is required.");

            try
            {
                var chunks = await _retrievalService.GetTopMatchesAsync(q);
                var context = string.Join("\n", chunks.Select(c => c.Text));

                var prompt = new StringBuilder();
                prompt.AppendLine("Use the following context to answer the question below:");
                prompt.AppendLine(context);
                prompt.AppendLine("\nQuestion:");
                prompt.AppendLine(q);

                var answer = await _embeddingService.GenerateAnswerAsync(prompt.ToString());
                return Ok(new { answer });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Answer generation failed: {ex.Message}");
            }
        }


    }
}

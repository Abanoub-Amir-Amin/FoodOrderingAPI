using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto review)
        {
            if (review == null)
            {
                return BadRequest("Review cannot be null.");
            }
            await reviewService.CreateReviewAsync(review);
            return Ok(review);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewById(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                return BadRequest("Review ID cannot be empty.");
            }
            var review = await reviewService.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpGet("getorderreviewbyorderid/{orderId}")]
        public async Task<IActionResult> GetReviewsByOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest("Order ID cannot be empty.");
            }
            var reviews = await reviewService.GetReviewsByOrderIdAsync(orderId);
            return Ok(reviews);
        }

        [HttpGet("getorderreviewbycustid/{customerId}")]
        public async Task<IActionResult> GetReviewsByCustomerId(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest("Customer ID cannot be null or empty.");
            }
            var reviews = await reviewService.GetReviewsByCustomerIdAsync(customerId);
            return Ok(reviews);
        }

        [HttpGet("getorderreviewbyrestid/{restaurantId}")]
        public async Task<IActionResult> GetReviewsByRestaurantId(string restaurantId)
        {
            if (string.IsNullOrEmpty(restaurantId))
            {
                return BadRequest("Restaurant ID cannot be null or empty.");
            }
            var reviews = await reviewService.GetReviewsByRestaurantIdAsync(restaurantId);
            return Ok(reviews);
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                return BadRequest("Review ID cannot be empty.");
            }
            await reviewService.DeleteReviewAsync(reviewId);
            return NoContent();
        }

        [HttpGet("getallreviews")]
        public async Task<IActionResult> GetAllReviewsForAdmin()
        {
            var reviews = await reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }


    }
}

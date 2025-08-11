using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;

namespace FoodOrderingAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo reviewRepo;

        public ReviewService(IReviewRepo reviewRepo)
        {
            this.reviewRepo = reviewRepo;
        }

        public async Task CreateReviewAsync(ReviewDto review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null.");
            }
            var reviewEntity = new Review
            {
                ReviewID = Guid.NewGuid(),
                CustomerID = review.CustomerID,
                RestaurantID = review.RestaurantID,
                OrderID = review.OrderID,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await reviewRepo.CreateReviewAsync(reviewEntity);
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new ArgumentException("Review ID cannot be empty.", nameof(reviewId));
            }
            await reviewRepo.DeleteReviewAsync(reviewId);
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await reviewRepo.GetAllReviewsAsync();
        }

        public Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new ArgumentException("Review ID cannot be empty.", nameof(reviewId));
            }
            var review = reviewRepo.GetReviewByIdAsync(reviewId);
            return review;
        }

        public Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.", nameof(customerId));
            }
            var reviews = reviewRepo.GetReviewsByCustomerIdAsync(customerId);
            return reviews;
        }

        public Task<IEnumerable<Review>> GetReviewsByOrderIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            var reviews = reviewRepo.GetReviewsByOrderIdAsync(orderId);
            return reviews;
        }

        public Task<IEnumerable<Review>> GetReviewsByRestaurantIdAsync(string restaurantId)
        {
            if (string.IsNullOrEmpty(restaurantId))
            {
                throw new ArgumentException("Restaurant ID cannot be null or empty.", nameof(restaurantId));
            }
            var reviews = reviewRepo.GetReviewsByRestaurantIdAsync(restaurantId);
            return reviews;
        }
    }
}

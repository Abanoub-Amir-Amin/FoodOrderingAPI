using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;

namespace FoodOrderingAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo reviewRepo;
        private readonly IOrderRepo orderRepo;
        public ReviewService(IReviewRepo reviewRepo, IOrderRepo orderRepo)
        {
            this.reviewRepo = reviewRepo;
            this.orderRepo = orderRepo;
        }

        public async Task CreateReviewAsync(ReviewDto review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null.");
            }
            var order = await orderRepo.GetOrderDetails(review.OrderID);
            if (order == null)
                throw new ArgumentException("there is no order with this id");
            var reviewEntity = new Review
            {
                ReviewID = Guid.NewGuid(),
                CustomerID = review.CustomerID,
                RestaurantID = order.RestaurantID,
                OrderID = review.OrderID,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.Now
            };
            await reviewRepo.CreateReviewAsync(reviewEntity);
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new ArgumentException("Review ID cannot be empty.", nameof(reviewId));
            }
            return await reviewRepo.DeleteReviewAsync(reviewId);
        }

        public async Task<IEnumerable<ReviewGetterDTO>> GetAllReviewsAsync()
        {
            List <ReviewGetterDTO> reviews = new();
            var temp = await reviewRepo.GetAllReviewsAsync();
            foreach (var review in temp)
            {
                reviews.Add(new ReviewGetterDTO
                {
                    ReviewID = review.ReviewID,
                    CustomerID = review.CustomerID,
                    RestaurantID = review.RestaurantID,
                    OrderID = review.OrderID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.Customer?.User?.UserName ?? "Unknown"
                });
            }
            return reviews;
        }

        public async Task<ReviewGetterDTO> GetReviewByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
            {
                throw new ArgumentException("Review ID cannot be empty.", nameof(reviewId));
            }
            var review = await reviewRepo.GetReviewByIdAsync(reviewId);
            ReviewGetterDTO reviewGetter = new ReviewGetterDTO
            {
                ReviewID = review.ReviewID,
                CustomerID = review.CustomerID,
                RestaurantID = review.RestaurantID,
                OrderID = review.OrderID,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UserName = review.Customer?.User?.UserName ?? "Unknown"
            };

            return reviewGetter;
        }

        public async Task<IEnumerable<ReviewGetterDTO>> GetReviewsByCustomerIdAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.", nameof(customerId));
            }
            var temp = await reviewRepo.GetReviewsByCustomerIdAsync(customerId);
            List<ReviewGetterDTO> reviews = new();
            foreach (var review in temp)
            {
                reviews.Add(new ReviewGetterDTO
                {
                    ReviewID = review.ReviewID,
                    CustomerID = review.CustomerID,
                    RestaurantID = review.RestaurantID,
                    OrderID = review.OrderID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.Customer?.User?.UserName ?? "Unknown"
                });
            }
            return reviews;
        }

        public async Task<IEnumerable<ReviewGetterDTO>> GetReviewsByOrderIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));
            }
            var temp = await reviewRepo.GetReviewsByOrderIdAsync(orderId);
            List<ReviewGetterDTO> reviews = new();
            foreach (var review in temp)
            {
                reviews.Add(new ReviewGetterDTO
                {
                    ReviewID = review.ReviewID,
                    CustomerID = review.CustomerID,
                    RestaurantID = review.RestaurantID,
                    OrderID = review.OrderID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.Customer?.User?.UserName ?? "Unknown"
                });
            }
            return reviews;
        }

        public async Task<IEnumerable<ReviewGetterDTO>> GetReviewsByRestaurantIdAsync(string restaurantId)
        {
            if (string.IsNullOrEmpty(restaurantId))
            {
                throw new ArgumentException("Restaurant ID cannot be null or empty.", nameof(restaurantId));
            }
            var temp = await reviewRepo.GetReviewsByRestaurantIdAsync(restaurantId);
            List<ReviewGetterDTO> reviews = new();
            foreach (var review in temp)
            {
                reviews.Add(new ReviewGetterDTO
                {
                    ReviewID = review.ReviewID,
                    CustomerID = review.CustomerID,
                    RestaurantID = review.RestaurantID,
                    OrderID = review.OrderID,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    UserName = review.Customer?.User?.UserName ?? "Unknown"
                });
            }
            return reviews;
        }
    }
}

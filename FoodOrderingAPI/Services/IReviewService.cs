using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<IEnumerable<Review>> GetReviewsByOrderIdAsync(Guid orderId);
        Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId);
        Task<IEnumerable<Review>> GetReviewsByRestaurantIdAsync(string restaurantId);
        Task CreateReviewAsync(ReviewDto review);
        Task DeleteReviewAsync(Guid reviewId);
    }
}

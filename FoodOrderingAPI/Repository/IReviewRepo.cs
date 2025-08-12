using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IReviewRepo
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<IEnumerable<Review>> GetReviewsByOrderIdAsync(Guid orderId);
        Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId);
        Task<IEnumerable<Review>> GetReviewsByRestaurantIdAsync(string restaurantId);
        Task CreateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(Guid reviewId);
    }
}

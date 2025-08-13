using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewGetterDTO>> GetAllReviewsAsync();
        Task<ReviewGetterDTO> GetReviewByIdAsync(Guid reviewId);
        Task<IEnumerable<ReviewGetterDTO>> GetReviewsByOrderIdAsync(Guid orderId);
        Task<IEnumerable<ReviewGetterDTO>> GetReviewsByCustomerIdAsync(string customerId);
        Task<IEnumerable<ReviewGetterDTO>> GetReviewsByRestaurantIdAsync(string restaurantId);
        Task CreateReviewAsync(ReviewDto review);
        Task<bool> DeleteReviewAsync(Guid reviewId);
    }
}

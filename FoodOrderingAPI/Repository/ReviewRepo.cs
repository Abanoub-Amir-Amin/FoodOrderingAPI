using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class ReviewRepo: IReviewRepo
    {
        private readonly ApplicationDBContext _context;
        public ReviewRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.ToListAsync();
        }
        public async Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }
        public async Task CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                var isDeleted = _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                Console.WriteLine(isDeleted);
                return isDeleted != null;
            }
            return false;
        }

        public async Task<IEnumerable<Review>> GetReviewsByOrderIdAsync(Guid orderId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.OrderID == orderId)
                .ToListAsync();
            return reviews;
        }

        public async Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CustomerID == customerId)
                .ToListAsync();
            return reviews;
        }

        public async Task<IEnumerable<Review>> GetReviewsByRestaurantIdAsync(string restaurantId)
        {
            var reviews = await _context.Reviews.Where(r => r.RestaurantID == restaurantId).ToListAsync();
            return reviews;
        }

    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
namespace FoodOrderingAPI.Interfaces
{
    public interface ICustomerRepo
    {
        public Task<Customer> GetById(string id);
        public Task<List<Customer>> GetAll();
        public Task Add(Customer customer);
        public Task<Customer> Update(Customer customer);
        public Task<Customer> Delete(string id);
        public Task Save();
        public Task<Customer> GetByEmail(string email);
        public Task<Customer> GetByUsername(string UserName);
        //public Task<IdentityResult> Register(RegisterCustomerDTO dto);
        //public Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerReviewsAsync(int customerId);
        //public Task<IEnumerable<RewardHistory>> GetCustomerRewardssAsync(int customerId);
        //public Task<ShoppingCart> GetShoppingCard(int customerId);


    }
}

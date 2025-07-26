using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;
namespace FoodOrderingAPI.Interfaces
{
    public interface ICustomerRepo
    {
        public Task<CustomerDTO> GetById(string id);
        public Task<List<CustomerDTO>> GetAll();
        public Task Add(Customer customer);
        public Task<bool> Update(string CustomerId, UpdateCustomerDTO customer);
        public Task<bool> Delete(string id);
        public Task Save();
        public Task<CustomerDTO> GetByEmail(string email);
        public Task<CustomerDTO> GetByUsername(string UserName);
        public Task<IdentityResult> Register(RegisterCustomerDTO dto);
        //public Task<CustomerRepo> Authenticate(LoginDTO customer);
        //public Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId);
        //public Task<IEnumerable<Review>> GetCustomerReviewsAsync(int customerId);
        //public Task<IEnumerable<RewardHistory>> GetCustomerRewardssAsync(int customerId);
        //public Task<ShoppingCart> GetShoppingCard(int customerId);


    }
}

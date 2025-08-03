using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodOrderingAPI.Services
{
    public interface ICustomerServices
    {
        public Task<CustomerDTO> GetCusomerDashboardDataById(string id);
        public Task<List<CustomerDTO>> GetAll();
        public Task<bool> UpdateCustomer(string id,UpdateCustomerDTO customerDto);
        public Task<bool> DeleteCustomer(string id);
        public Task Save();

        public Task<CustomerDTO> GetCusomerDashboardDataByEmail(string email);
        public Task<CustomerDTO> GetCusomerDashboardDataByUserName(string UserName);
        public Task<IdentityResult> Register(RegisterCustomerDTO dto);


    }
}

using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Interfaces
{
    public interface IAddressRepo
    {
        public Task<List<Address>> GetAllAddresses(string UserId);
        public Task<Address> GetAddress(Guid addressId);
        public Task<Address> getDafaultAddress(string customerId);
        public Task<Address> MakeDefault(Guid AddressId);
        public Task<Address> Add(string UserId, AddressDTO address);
        public Task<Address> AddToOrder(Address addressdto);
        public Task<bool> Update(Guid AddressId,AddressDTO address);
        public Task<bool> Delete(Guid AddressId);
        public Task Save();


    }
}

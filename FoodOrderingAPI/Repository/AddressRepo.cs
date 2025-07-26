using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Interfaces;
using FoodOrderingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingAPI.Repository
{
    public class AddressRepo : IAddressRepo
    {
        ApplicationDBContext dbContext;
        public AddressRepo(ApplicationDBContext dbContext) {
            this.dbContext = dbContext;
        }
        public Task<List<Address>> GetAllAddresses(string Username)
        {
            return dbContext.Addresses
                .Where(A => A.Customer.User.UserName == Username)
                .ToListAsync();
        }
        public async Task<Address> GetAddress(Guid addressId)
        {
           Address ?address= await dbContext.Addresses
                .FirstOrDefaultAsync(A => A.AddressID == addressId);
            return address;
        }
        public async Task Add(string Username, AddressDTO addressdto)
        {
            Customer customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.User.UserName == Username);
            Address address = new Address()
            {
                Label = addressdto.Label,
                Street = addressdto.Street,
                City = addressdto.City,
                LatLng = addressdto.LatLng,
                IsDefault = addressdto.IsDefault,
                CustomerID = customer.CustomerID
            };
            dbContext.Addresses.Add(address);
            //dbContext.SaveChanges();
        }
        public async Task<bool> Update(Guid AddressId, AddressDTO addressdto)
        {
            Address address = await GetAddress(AddressId);
            if (address == null) { return false; }
            address.Street = addressdto.Street;
            address.City = addressdto.City;
            address.LatLng = addressdto.LatLng;
            address.IsDefault = addressdto.IsDefault;
            //dbContext.SaveChanges();
            return true;
        }
        public async Task<bool> Delete(Guid AddressId)
        {
            Address address = await GetAddress(AddressId);
            if (address == null) { return false; }
            dbContext.Addresses.Remove(address);
            //dbContext.SaveChanges();
            return true;
        }
        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}

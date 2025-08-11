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
        public async Task<Address> MakeDefault(Guid AddressId)
        {
            
            var address= await dbContext.Addresses
                .Include(A => A.Customer)
                .FirstOrDefaultAsync(A => A.AddressID == AddressId);
            if (address != null)
            {
                Customer customer = address.Customer;
                foreach (var addr in customer.Addresses)
                {
                    addr.IsDefault = false;
                }

                // نخلي العنوان المختار هو بس اللي IsDefault = true
                address.IsDefault = true;

                await dbContext.SaveChangesAsync();
                return address;
            }
            else 
                { return null; }
        }
        public async Task<Address> getDafaultAddress(string customerId)
        {
            return await dbContext.Addresses.FirstOrDefaultAsync(A => A.CustomerID == customerId && A.IsDefault);
        }
        public async Task<Address> Add(string Username, AddressDTO addressdto)
        {
            Customer customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.User.UserName == Username);
            bool isDefault = false;
            if(customer.Addresses==null)
                isDefault = true;
            Address address = new Address()
            {
                Label = addressdto.Label,
                Street = addressdto.Street,
                City = addressdto.City,
                Longitude = addressdto.Longitude,
                Latitude = addressdto.Latitude,
                IsDefault = isDefault,
                CustomerID = customer.CustomerID
            };
            dbContext.Addresses.Add(address);
            return address;
            //dbContext.SaveChanges();
        }
        public async Task<bool> Update(Guid AddressId, AddressDTO addressdto)
        {
            Address address = await GetAddress(AddressId);
            if (address == null) { return false; }
            address.Street = addressdto.Street;
            address.City = addressdto.City;
            address.Longitude = addressdto.Longitude;
            address.Latitude = addressdto.Latitude;

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

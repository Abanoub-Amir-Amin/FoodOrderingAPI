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
        public Task<List<Address>> GetAllAddresses(string UserId)
        {
            return dbContext.Addresses
                .Where(A => A.Customer.CustomerID == UserId)
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
            var allAdresses = await dbContext.Addresses.ToListAsync();
            var address= await dbContext.Addresses
                .Include(A => A.Customer)
                .FirstOrDefaultAsync(A => A.AddressID == AddressId);
            if (address != null)
            {
                Customer customer = address.Customer;
                foreach (var addr in allAdresses)
                {
                    if(addr.CustomerID==address.CustomerID)
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
        public async Task<Address> Add(string UserId, AddressDTO addressdto)
        {
            if (addressdto.Latitude == 0 && addressdto.Longitude==0)
                throw new ArgumentException("Invalid map selection: Latitude and Longitude are required.");
            if (string.IsNullOrEmpty(addressdto.Street) || string.IsNullOrEmpty(addressdto.City))
                throw new ArgumentException("street and city are required");
            
            var foundaddress = await dbContext.Addresses
                .Where(a => a.CustomerID == UserId 
                && Math.Round(a.Latitude,4) ==Math.Round(addressdto.Latitude,4) 
                &&Math.Round(a.Longitude) == Math.Round(addressdto.Longitude))
                .ToListAsync();
            if (foundaddress.Count > 0)
                throw new ArgumentException("this address added before");
            Customer customer = await dbContext.Customers.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.CustomerID == UserId);
            bool isDefault = false;
            if(customer.Addresses==null || customer.Addresses?.Count==0)
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
            string customerId = address.CustomerID;
            address.CustomerID = null;
            if (address.IsDefault)
            {
                var newDefaultAddress = await dbContext.Addresses.Where(a => a.CustomerID==customerId).FirstOrDefaultAsync();
                if(newDefaultAddress!=null)
                    newDefaultAddress.IsDefault = true;
            }
            //dbContext.SaveChanges();
            return true;
        }
        public async Task Save()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
